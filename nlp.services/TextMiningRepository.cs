using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;

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
            _logger = logger ?? throw new NlpException(HttpStatusCode.InternalServerError, nameof(logger));
            _models = models ?? throw new NlpException(HttpStatusCode.InternalServerError, nameof(models));
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
                    if (!wordCount.ContainsKey(x))
                        wordCount.Add(x, 1);
                    else if (wordCount.ContainsKey(x))
                        wordCount[x]++;
                });
            sw.Stop();

            _logger.LogInformation($"text mining algorithm took {sw.Elapsed.TotalMilliseconds * 1000} µs (microseconds)");

            return wordCount;
        }

        public object Stem(string Content)
        {
            var sw = new Stopwatch();
            var stems = new List<StemInfo>();

            sw.Start();
            Content.Split(_models.DefaultDelimiters)
                .Where(x => !_models.DetaulfStopWords.Contains(x))
                .ToList()
                .ForEach(x =>
                {
                    var xStem = x.Stem();
                    var stem = stems.FirstOrDefault(x => x.Stem == xStem);

                    if (stem == null)
                        stems.Add(new StemInfo()
                        {
                            Stem = xStem,
                            Originals = new List<string>() { x }
                        });
                    else
                        stem.Originals.Add(x);

                });
            sw.Stop();

            _logger.LogInformation($"stemming algorithm took {sw.Elapsed.TotalMilliseconds * 1000} µs (microseconds)");

            return stems;
        }

        private class StemInfo
        {
            public string Stem { get; set; }
            public ICollection<string> Originals { get; set; } = new List<string>();
        }
    }
}
