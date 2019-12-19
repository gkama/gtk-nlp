using System;

using Microsoft.Extensions.Logging;

namespace nlp.services
{
    public class NlpRepository : INlpRepository
    {
        private readonly ILogger<NlpRepository> _logger;

        public NlpRepository(ILogger<NlpRepository> logger)
        {
            _logger = logger;
        }
    }
}
