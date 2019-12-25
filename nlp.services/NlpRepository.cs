using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
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

            var detailsSplit = modelSettings.Model.Details
                ?.Split(modelSettings.Delimiters, StringSplitOptions.RemoveEmptyEntries);

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
                        Model = SerializeModel(model)
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

        public T SerializeModel(JsonElement Model)
        {
            var model = new T()
            {
                Id = Model.GetProperty("id").GetString(),
                Name = Model.GetProperty("name").GetString(),
                Details = Model.GetProperty("details").GetString(),
            };

            var baseModelChildren = Model.GetProperty("children");
            var childrenList = new List<JsonElement>();
            for (int i = 0; i < baseModelChildren.GetArrayLength(); i++)
                childrenList.Add(baseModelChildren[i]);

            var stack = new Stack<JsonElement>(childrenList);
            while (stack.Any())
            {
                var child = stack.Pop();

                model.Children.Add(new T()
                {
                    Id = child.GetProperty("id").GetString(),
                    Name = child.GetProperty("name").GetString(),
                    Details = child.GetProperty("details").GetString(),
                });

                if (child.GetProperty("children").GetArrayLength() > 0)
                {
                    var childModels = child.GetProperty("children");
                    for (int i = 0; i < childModels.GetArrayLength(); i++)
                        stack.Push(childModels[i]);
                }
            }

            return model;
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
