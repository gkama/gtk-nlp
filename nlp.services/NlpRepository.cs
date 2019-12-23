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
            if (Parse(Request))
            {
                Console.WriteLine("Passed");
            }

            return "";
        }

        public bool Parse(dynamic Request)
        {
            var jRequest = (JsonElement)Request;
            try
            {
                var model = jRequest.GetProperty("model").ToObject<Model>();
                var delimiters = jRequest.GetProperty("delimiters").ToObject<string[]>();
                var stopWords = jRequest.GetProperty("stopWords").ToObject<string[]>();
            }
            catch (KeyNotFoundException)
            {
                throw new NlpException(HttpStatusCode.InternalServerError,
                    $"`model`, `stopWords` and `delimiter` keys are required. please include them in your JSON payload");
            }
            catch (Exception e)
            {
                throw new NlpException(HttpStatusCode.InternalServerError,
                    $"couldn't parse the dynamic request. error={e.Message}");
            }

            return true;
        }
    }
}
