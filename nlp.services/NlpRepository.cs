using System;

using Microsoft.Extensions.Logging;

namespace nlp.services
{
    public class NlpRepository<T> : INlpRepository<T>
        where T : class
    {
        private readonly ILogger<NlpRepository<T>> _logger;

        public NlpRepository(ILogger<NlpRepository<T>> logger)
        {
            _logger = logger;
        }
    }
}
