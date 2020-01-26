using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Diagnostics;
using System.Linq;
using System.Net;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;

using nlp.data;
using nlp.data.text;
using nlp.services.text;

namespace nlp.services
{
    public class NlpRepository<T> : INlpRepository<T>
        where T : IModel<T>, new()
    {
        private readonly ILogger<NlpRepository<T>> _logger;
        private readonly IMemoryCache _cache;
        private readonly ITextMiningRepository<T> _txtrepo;
        private readonly Models<T> _models;

        private ICollection<ICategory> _categories { get; set; } = new List<ICategory>();

        public NlpRepository(ILogger<NlpRepository<T>> logger, IMemoryCache cache, ITextMiningRepository<T> txtrepo, Models<T> models)
        {
            _logger = logger ?? throw new NlpException(HttpStatusCode.InternalServerError, nameof(logger));
            _cache = cache ?? throw new NlpException(HttpStatusCode.InternalServerError, nameof(cache));
            _txtrepo = txtrepo ?? throw new NlpException(HttpStatusCode.InternalServerError, nameof(txtrepo));
            _models = models ?? throw new NlpException(HttpStatusCode.InternalServerError, nameof(models));
        }

        public INlpResponse Categorize(INlpRequest<T> Request, string Id = null, bool Summarize = false)
        {
            var modelSettings = Parse(Request, Id);

            var content = Summarize
                ? _txtrepo.Summarize(Request.Content)
                : Request.Content;

            var models = new Stack<T>(new List<T>() { modelSettings.Model });
            var delimiters = modelSettings.Delimiters
                .Union(_models.DefaultDelimiters)
                .ToArray();
            var sw = new Stopwatch();

            sw.Start();
            while (models.Any())
            {
                var model = models.Pop() as IModel<T>;
                var detailsArray = model.Details
                    ?.Split(delimiters);

                if (detailsArray != null)
                {
                    Tokenize(content, delimiters, modelSettings.StopWords)
                        .ToList()
                        .ForEach(x =>
                        {
                            BinarySearchDetails(x, detailsArray, delimiters, model);
                        });

                    detailsArray.Where(x => x.Contains(' '))
                        .ToList()
                        .ForEach(x =>
                        {
                            if (content.Contains(x))
                                _categories.AddCategory(model.Name, x);
                        });
                }

                if (model.Children.Any())
                    model.Children
                        .ToList()
                        .ForEach(x =>
                        {
                            models.Push(x);
                        });
            }
            sw.Stop();

            _logger.LogInformation($"categorization algorithm took {sw.Elapsed.TotalMilliseconds * 1000} µs (microseconds)");

            return new NlpResponse(_categories, Request.Content.Length)
            {
                Summarized = Summarize,
                SummarizedLength = Summarize ? content.Length : null as int?
            };
        }

        public IModelSettings<T> Parse(INlpRequest<T> Request, string Id = null)
        {
            if (Id != null)
                return GetModelSettingsByModelId(Id);

            if (Request == null)
                throw new NlpException(HttpStatusCode.BadRequest,
                    $"request cannot be empty. please fix your JSON payload");
            else if (Request.Content == null)
                throw new NlpException(HttpStatusCode.BadRequest,
                    $"'content' key is required. please include it in your JSON payload");
            else if (Request.Content.Length > 100000)
                throw new NlpException(HttpStatusCode.BadRequest,
                    $"'content' length={Request?.Content?.Length} is too big. it must be less than 100,000 characters");

            if (Request.Model != null)
            {
                return new ModelSettings<T>()
                {
                    Id = Guid.NewGuid().ToString(),
                    StopWords = Request.StopWords,
                    Delimiters = Request.Delimiters.Select(char.Parse).ToArray(),
                    Model = _cache.GetOrCreate(Request.Model.PublicKey, e =>
                        {
                            e.SlidingExpiration = TimeSpan.FromSeconds(_models.TenMinutesCacheTimeSpan);

                            return Request.Model;
                        })
                };
            }
            else if (Request.ModelId != null
                && Request.ModelName != null
                && Request.ModelDetails != null)
            {
                return new ModelSettings<T>()
                {
                    Id = Guid.NewGuid().ToString(),
                    StopWords = Request.StopWords,
                    Delimiters = Request.Delimiters.Select(char.Parse).ToArray(),
                    Model = _cache.GetOrCreate(Request.ModelId, e =>
                        {
                            e.SlidingExpiration = TimeSpan.FromSeconds(_models.TenMinutesCacheTimeSpan);

                            return new T()
                            {
                                Id = Request.ModelId,
                                Name = Request.ModelName,
                                Details = Request.ModelDetails
                            };
                        })
                };
            }
            else
                throw new NlpException(HttpStatusCode.BadRequest,
                    $"not enough information given to parse the JSON payload");
        }

        public IEnumerable<string> Tokenize(string Content, char[] Delimiters, string[] StopWords)
        {
            if (string.IsNullOrWhiteSpace(Content)) return Enumerable.Empty<string>();

            return Content.Split(Delimiters, StringSplitOptions.RemoveEmptyEntries)
                .Except(StopWords, StringComparer.OrdinalIgnoreCase)
                .AsEnumerable();
        }

        private void BinarySearchDetails(string Value, string[] DetailsArray, char[] Delimiters, IModel<T> Model)
        {
            DetailsArray.OrderBy(x => x);

            var low = 0;
            var mid = 0;
            var high = DetailsArray.Count() - 1;

            while (low <= high)
            {
                mid = (low + high) / 2;

                if (string.Compare(Value, DetailsArray[mid], StringComparison.OrdinalIgnoreCase) < 0)
                    high = mid - 1;
                else if (string.Compare(Value, DetailsArray[mid], StringComparison.OrdinalIgnoreCase) > 0)
                    low = mid + 1;
                else
                {
                    _categories.AddCategory(Model.Name, Value);
                    break;
                }
            }
        }

        public IEnumerable<T> GetModels()
        {
            return _cache.GetOrCreate(_models.All, e =>
            {
                e.SlidingExpiration = TimeSpan.FromSeconds(_models.DefaultCacheTimeSpan);

                return _models.All;
            });
        }
        public IModel<T> GetModel(string Id)
        {
            var model = _cache.GetOrCreate(Id, e =>
            {
                e.SlidingExpiration = TimeSpan.FromSeconds(_models.DefaultCacheTimeSpan);

                return _models.All
                    .FirstOrDefault(x => x.Id == Id);
            });

            if (model == null)
            {
                Guid.TryParse(Id, out Guid PublicKey);

                return GetModel(PublicKey);
            }

            return model;
        }
        public IModel<T> GetModel(Guid PublicKey)
        {
            return _cache.GetOrCreate(PublicKey, e =>
            {
                e.SlidingExpiration = TimeSpan.FromSeconds(_models.DefaultCacheTimeSpan);

                return _models.All
                    .FirstOrDefault(x => x.PublicKey == PublicKey);
            });
        }
        public IModel<T> GetAnyModel(string Id)
        {
            return _cache.GetOrCreate(Id, e =>
            {
                e.SlidingExpiration = TimeSpan.FromSeconds(_models.DefaultCacheTimeSpan);

                var models = new Stack<T>(_models.All);

                while (models.Any())
                {
                    var model = models.Pop() as IModel<T>;

                    if (model.Id == Id) return model;

                    if (model.Children.Any())
                        model.Children
                            .ToList()
                            .ForEach(x =>
                            {
                                models.Push(x);
                            });
                }

                return null;
            });
        }
        public IEnumerable<IModelSettings<T>> GetModelsSettings()
        {
            return _cache.GetOrCreate(_models.Settings, e =>
            {
                e.SlidingExpiration = TimeSpan.FromSeconds(_models.DefaultCacheTimeSpan);

                return _models.Settings;
            });
        }
        public IModelSettings<T> GetModelSettings(string Id)
        {
            return _cache.GetOrCreate(Id, e =>
            {
                e.SlidingExpiration = TimeSpan.FromSeconds(_models.DefaultCacheTimeSpan);

                return _models.Settings
                    .FirstOrDefault(x => x.Id == Id);
            });
        }
        public IModelSettings<T> GetModelSettingsByModelId(string Id)
        {
            return _cache.GetOrCreate(Id, e =>
            {
                e.SlidingExpiration = TimeSpan.FromSeconds(_models.DefaultCacheTimeSpan);

                return _models.Settings
                    .FirstOrDefault(x => x.Model.Id == Id);
            });
        }

        public T AddModel(dynamic Request)
        {
            var jsonRequest = (JsonElement)Request;
            var model = jsonRequest.GetProperty("model")
                .GetRawText()
                .DeserializeSelfReferencing<T>();

            _cache.Set(model.PublicKey, model, DateTimeOffset.Now.AddSeconds(_models.DefaultCacheTimeSpan));

            return model;
        }
        public T AddModel(INlpRequest<T> Request)
        {
            _cache.Set(Request.Model.PublicKey, Request.Model, DateTimeOffset.Now.AddSeconds(_models.DefaultCacheTimeSpan));

            return Request.Model;
        }
        public T AddModel(T Model)
        {
            _cache.Set(Model.PublicKey, Model, DateTimeOffset.Now.AddSeconds(_models.DefaultCacheTimeSpan));

            return Model;
        }

        public void DeleteModel(string Id)
        {
            var model = GetModel(Id);

            try {  _cache.Remove(model?.Id); } catch (Exception) { throw new NlpException(HttpStatusCode.BadRequest, $"couldn't delete model with id={Id}"); }
            try { _cache.Remove(model?.PublicKey); } catch (Exception) { throw new NlpException(HttpStatusCode.BadRequest, $"couldn't delete model with public key={model?.PublicKey}"); }
        }
        public void DeleteModel(string Id, Guid PublicKey)
        {
            try { _cache.Remove(Id); } catch (Exception) { throw new NlpException(HttpStatusCode.BadRequest, $"couldn't delete model with id={Id}"); }
            try { _cache.Remove(PublicKey); } catch (Exception) { throw new NlpException(HttpStatusCode.BadRequest, $"couldn't delete model with public key={PublicKey}"); }
        }

        public object CategorizeSample()
        {
            var requestContent = "This is a sample content passed to the Categorize endpoint. What it'll try and match is the financial model. Specificall, the Vanguard index funds. Such index funds are VBMFX and VTSAX.";
            var nlpRequest = new NlpRequest<T>()
            {
                Content = requestContent
            };

            return _cache.GetOrCreate(requestContent, e =>
            {
                e.SlidingExpiration = TimeSpan.FromSeconds(_models.DefaultCacheTimeSpan);

                return new
                {
                    content = requestContent,
                    categorization = Categorize(nlpRequest, _models.Financial.Id),
                    model = _models.Financial
                };
            });
        }

        public object GetNlpRequestSchema()
        {
            var requestProperties = typeof(NlpRequest<T>).GetType()
                .GetProperties()
                .Select(x => new
                {
                    name = x.Name,
                    type = x.PropertyType.Name
                });

            var modelProperties = typeof(T)
                .GetProperties()
                .Select(x => new
                {
                    name = x.Name,
                    type = x.PropertyType.Name
                });

            return new
            {
                request = requestProperties,
                model = modelProperties
            };
        }
    }
}
