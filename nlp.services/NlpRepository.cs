using System;

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
            _logger = logger;
        }

        public string Categorize()
        {
            return "";
        }
    }
}
