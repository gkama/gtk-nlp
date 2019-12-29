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

        public ICollection<object> _categories { get; set; } = new List<object>();

        public NlpRepository(ILogger<NlpRepository<T>> logger, Models<T> models)
        {
            _logger = logger ?? throw new NlpException(HttpStatusCode.InternalServerError, nameof(logger));
            _models = models ?? throw new NlpException(HttpStatusCode.InternalServerError, nameof(models));
        }

        public object Categorize(dynamic Request)
        {
            var modelSettings = (IModelSettings<T>)Parse(Request);
            var content = ((JsonElement)Request)
                .GetProperty("content")
                .GetString();

            if (content.Length > modelSettings.StopWordsLength)
            {
                var models = new Stack<T>(new List<T>() { modelSettings.Model });
                var delimiters = modelSettings.Delimiters
                    .Union(_models.DefaultDelimiters)
                    .ToArray();
                var sw = new Stopwatch();

                sw.Start();
                while (models.Any())
                {
                    var model = models.Pop() as IModel<T>;

                    Tokenize(content, modelSettings)
                        .ToList()
                        .ForEach(x =>
                        {
                            BinarySearchDetails(x, delimiters, model);
                        });

                    //categorization of phrases
                    //grab all details that contain spaces and add it to the categories
                    var detailsArray = model.Details.Split(delimiters)
                        .ToList();
                    detailsArray
                        .Where(x => x.Contains(' '))
                        .ToList()
                        .ForEach(x =>
                        {
                            if (content.Contains(x))
                                _categories.Add(new
                                {
                                    name = model.Name,
                                    value = x
                                });
                        });

                    if (model.Children.Any())
                        model.Children.ToList().ForEach(x =>
                        {
                            models.Push(x);
                        });
                }
                sw.Stop();
                _logger.LogInformation($"categorization algorithm took {sw.Elapsed.TotalMilliseconds * 1000} (microseconds)");

                return _categories;
            }
            else
                Console.WriteLine("don't tokenize");

            return modelSettings;
        }

        public IModelSettings<T> Parse(dynamic Request)
        {
            var jRequest = (JsonElement)Request;

            try
            {
                var delimiters = jRequest.GetProperty("delimiters").ToObject<string[]>();
                var stopWords = jRequest.GetProperty("stopWords").ToObject<string[]>();
                var content = jRequest.GetProperty("content").GetString();

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
                else if (modelId.ValueKind != JsonValueKind.Undefined)
                {
                    return GetModelSettings(modelId.GetString());
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

        public IEnumerable<string> Tokenize(string Content, IModelSettings<T> Settings)
        {
            if (string.IsNullOrWhiteSpace(Content)) return Enumerable.Empty<string>();

            var delimiters = Settings.Delimiters
                .Union(_models.DefaultDelimiters)
                .ToArray();

            var words = Content.Split(delimiters,
                StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            var found = new List<string>();

            words.ForEach(x =>
            {
                var xLower = x.ToLower();

                if (!Settings.StopWords.Contains(xLower))
                    found.Add(x);
            });

            return found.AsEnumerable();
        }

        private void BinarySearchDetails(string Value, char[] Delimiters, IModel<T> Model)
        {
            var low = 0;
            var mid = 0;
            var detailsArray = Model.Details.Split(Delimiters) as string[];
            Array.Sort(detailsArray);
            var high = detailsArray.Count() - 1;

            while (low <= high)
            {
                mid = (low + high) / 2;

                if (string.Compare(Value, detailsArray[mid], StringComparison.OrdinalIgnoreCase) < 0)
                    high = mid - 1;
                else if (string.Compare(Value, detailsArray[mid], StringComparison.OrdinalIgnoreCase) > 0)
                    low = mid + 1;
                else
                {
                    _categories.Add(new
                    {
                        name = Model.Name,
                        value = Value
                    });
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
    }
}
