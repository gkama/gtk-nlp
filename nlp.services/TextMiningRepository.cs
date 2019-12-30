using System;
using System.Collections.Generic;
using System.Linq;

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

        public object Mine(string Content)
        {
            var wordCount = new Dictionary<string, int>();
            Content.Split(_models.DefaultDelimiters)
                .ToList()
                .ForEach(x =>
                {
                    if (!wordCount.ContainsKey(x))
                        wordCount.Add(x, 1);
                    else if (wordCount.ContainsKey(x))
                        wordCount[x]++;
                });

            return wordCount;
        }
    }
}
