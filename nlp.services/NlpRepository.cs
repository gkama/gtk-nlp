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
        where T : IModel<T>
    {
        private readonly ILogger<NlpRepository<T>> _logger;

        public NlpRepository(ILogger<NlpRepository<T>> logger)
        {
            _logger = logger ?? throw new NlpException(HttpStatusCode.InternalServerError, nameof(logger));
        }

        public string Categorize(dynamic Request)
        {
            var model = Parse(Request);

            if (model != null)
            {
                return JsonSerializer.Serialize(model);
            }

            return null;
        }

        public IModel<Model> Parse(dynamic Request)
        {
            var jRequest = (JsonElement)Request;

            try
            {
                var delimiters = jRequest.GetProperty("delimiters").ToObject<string[]>();
                var stopWords = jRequest.GetProperty("stopWords").ToObject<string[]>();

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
                    return model.ToObject<Model>();
                }
                else if (modelId.ValueKind != JsonValueKind.Undefined
                    && modelName.ValueKind != JsonValueKind.Undefined
                    && modelDetails.ValueKind != JsonValueKind.Undefined)
                {
                    return new Model()
                    {
                        Id = modelId.GetString(),
                        Name = modelName.GetString(),
                        Details = modelDetails.GetString()
                    };
                }
                else
                    throw new NlpException(HttpStatusCode.BadRequest,
                        $"not enough information given in the JSON payload");
            }
            catch (KeyNotFoundException)
            {
                throw new NlpException(HttpStatusCode.InternalServerError,
                    $"``stopWords` and `delimiter` keys are required. please include them in your JSON payload");
            }
            catch (Exception e)
            {
                throw new NlpException(HttpStatusCode.InternalServerError,
                    $"couldn't parse the dynamic request. error={e.Message}");
            }
        }
    }
}
