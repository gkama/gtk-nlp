using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Logging;

using nlp.data;

namespace nlp.services
{
    public class TextMiningRepository<T> : ITextMiningRepository<T>
        where T : IModel<T>, new()
    {
        private readonly ILogger<TextMiningRepository<T>> _logger;
        private readonly Models<T> _models;

        public TextMiningRepository(ILogger<TextMiningRepository<T>> logger, Models<T> models)
        {
            _logger = logger;
            _models = models;
        }
    }
}
