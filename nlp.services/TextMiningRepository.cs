using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var sw = new Stopwatch();
            var wordCount = new Dictionary<string, int>();

            sw.Start();
            Content.Split(_models.DefaultDelimiters)
                .Where(x => !_models.DetaulfStopWords.Contains(x))
                .ToList()
                .ForEach(x =>
                {
                    var xStem = x.Stem();
                    if (!wordCount.ContainsKey(xStem))
                        wordCount.Add(xStem, 1);
                    else if (wordCount.ContainsKey(xStem))
                        wordCount[xStem]++;
                });
            sw.Stop();

            _logger.LogInformation($"text mining algorithm took {sw.Elapsed.TotalMilliseconds * 1000} µs (microseconds)");

            return wordCount;
        }
    }
}
