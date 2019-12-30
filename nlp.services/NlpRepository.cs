using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Diagnostics;
using System.Linq;
using System.Net;

using Microsoft.Extensions.Logging;

using nlp.data;

namespace nlp.services
{
    public class NlpRepository<T> : INlpRepository<T>
        where T : IModel<T>, new()
    {
        private readonly ILogger<NlpRepository<T>> _logger;
        private readonly Models<T> _models;

        public ICollection<ICategory> _categories { get; set; } = new List<ICategory>();

        public NlpRepository(ILogger<NlpRepository<T>> logger, Models<T> models)
        {
            _logger = logger ?? throw new NlpException(HttpStatusCode.InternalServerError, nameof(logger));
            _models = models ?? throw new NlpException(HttpStatusCode.InternalServerError, nameof(models));
        }

        public IEnumerable<ICategory> Categorize(dynamic Request, string Id = null)
        {
            var modelSettings = (IModelSettings<T>)Parse(Request, Id);
            var content = ((JsonElement)Request)
                .GetProperty("content")
                .GetString();

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
            _logger.LogInformation($"categorization algorithm took {sw.Elapsed.TotalMilliseconds * 1000} �s (microseconds)");

            return _categories;
        }

        public IModelSettings<T> Parse(dynamic Request, string Id = null)
        {
            var jRequest = (JsonElement)Request;

            try
            {
                var content = jRequest.GetProperty("content").GetString();

                if (Id != null)
                    return GetModelSettingsByModelId(Id);

                var delimiters = jRequest.GetProperty("delimiters").ToObject<string[]>();
                var stopWords = jRequest.GetProperty("stopWords").ToObject<string[]>();               

                if (content.Length > 10000)
                    throw new NlpException(HttpStatusCode.BadRequest,
                        $"'content' length={content.Length} is too big. it must be less than 10,000 characters");

                var model = new JsonElement();
                var modelId = new JsonElement();
                var modelName = new JsonElement();
                var modelDetails = new JsonElement();

                jRequest.TryGetProperty("model", out model);
                jRequest.TryGetProperty("modelId", out modelId);
                jRequest.TryGetProperty("modelName", out modelName);
                jRequest.TryGetProperty("modelDetails", out modelDetails);

                if (model.ValueKind != JsonValueKind.Undefined)
                {
                    return new ModelSettings<T>()
                    {
                        Id = Guid.NewGuid().ToString(),
                        StopWords = stopWords,
                        Delimiters = delimiters.Select(char.Parse).ToArray(),
                        Model = model.ToModel<T>()
                    };
                }
                else if (modelId.ValueKind != JsonValueKind.Undefined
                    && modelName.ValueKind != JsonValueKind.Undefined
                    && modelDetails.ValueKind != JsonValueKind.Undefined)
                {
                    return new ModelSettings<T>()
                    {
                        Id = Guid.NewGuid().ToString(),
                        StopWords = stopWords,
                        Delimiters = delimiters.Select(char.Parse).ToArray(),
                        Model = new T()
                        {
                            Id = modelId.GetString(),
                            Name = modelName.GetString(),
                            Details = modelDetails.GetString()
                        }
                    };
                }
                else
                    throw new NlpException(HttpStatusCode.BadRequest,
                        $"not enough information given to parse the JSON payload");
            }
            catch (KeyNotFoundException)
            {
                throw new NlpException(HttpStatusCode.InternalServerError,
                    $"'content', 'stopWords' and 'delimiters' keys are required. please include them in your JSON payload");
            }
            catch (Exception e)
            {
                throw new NlpException(HttpStatusCode.InternalServerError,
                    $"couldn't parse the dynamic request. error={e.Message}");
            }
        }

        public IEnumerable<string> Tokenize(string Content, char[] Delimiters, string[] StopWords)
        {
            if (string.IsNullOrWhiteSpace(Content)) return Enumerable.Empty<string>();

            var words = Content.Split(Delimiters,
                StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            var found = new List<string>();

            words.ForEach(x =>
            {
                if (!StopWords.Contains(x.ToLower()))
                    found.Add(x);
            });

            return found.AsEnumerable();
        }

        private void BinarySearchDetails(string Value, string[] DetailsArray, char[] Delimiters, IModel<T> Model)
        {
            var low = 0;
            var mid = 0;
            DetailsArray.OrderBy(x => x);
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
            return _models.All
                .Select(x => x.Model);
        }
        public IModel<T> GetModel(string Id)
        {
            var modelSetting = _models.All
                .FirstOrDefault(x => x.Model.Id == Id);

            if (modelSetting == null) return null;

            return modelSetting.Model;
        }
        public IEnumerable<IModelSettings<T>> GetModelsSettings()
        {
            return _models.All;
        }
        public IModelSettings<T> GetModelSettings(string Id)
        {
            return _models.All
                .FirstOrDefault(x => x.Id == Id);
        }
        public IModelSettings<T> GetModelSettingsByModelId(string Id)
        {
            return _models.All
                .FirstOrDefault(x => x.Model.Id == Id);
        }
    }
}
