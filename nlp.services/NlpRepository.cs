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
                var d = ((JsonElement)Request)
                  .GetProperty("details")
                  .GetString();
            }

            return "";
        }

        private bool Parse(dynamic Request)
        {
            var jsonRequest = (JsonElement)Request;
            try
            {
                var delimiter = jsonRequest.GetProperty("delimiter").GetString();
                var details = jsonRequest.GetProperty("details").GetString();
                var stopWords = jsonRequest.GetProperty("stopWords");               
            }
            catch (KeyNotFoundException)
            {
                throw new NlpException(HttpStatusCode.InternalServerError,
                    $"`details`, `stopWords` and `delimiter` keys are required. please include them in your JSON payload");
            }
            catch (Exception)
            {
                throw new NlpException(HttpStatusCode.InternalServerError,
                    $"couldn't parse the dynamic request");
            }

            return true;         
        }
    }
}
