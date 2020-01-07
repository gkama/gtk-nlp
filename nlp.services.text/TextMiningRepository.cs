using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Logging;

using nlp.data;

namespace nlp.services.text
{
    public class TextMiningRepository<T> : ITextMiningRepository<T>
        where T : IModel<T>, new()
    {
        private readonly ILogger<TextMiningRepository<T>> _logger;
        private readonly IStemmer _stemmer;
        private readonly Models<T> _models;

        public TextMiningRepository(ILogger<TextMiningRepository<T>> logger, IStemmer stemmer, Models<T> models)
        {
            _logger = logger ?? throw new NlpException(HttpStatusCode.InternalServerError, nameof(logger));
            _stemmer = stemmer ?? throw new NlpException(HttpStatusCode.InternalServerError, nameof(stemmer));
            _models = models ?? throw new NlpException(HttpStatusCode.InternalServerError, nameof(models));
        }

        public object Mine(string Content)
        {
            var sw = new Stopwatch();
            var wordCount = new Dictionary<string, int>();

            sw.Start();
            Content.Split(_models.DefaultDelimiters)
                .Where(x => !_models.DefaultStopWords.Contains(x))
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

        public IEnumerable<IStemmedWord> Stem(string Content)
        {
            var sw = new Stopwatch();
            var stems = new List<IStemmedWord>();

            sw.Start();
            Content.Split(_models.DefaultDelimiters)
                .Where(x => !_models.DefaultStopWords.Contains(x) && !string.IsNullOrEmpty(x))
                .ToList()
                .ForEach(x =>
                {
                    var stemmed = _stemmer.Stem(x);
                    if (!stems.Contains(stemmed))
                        stems.Add(stemmed);
                });
            sw.Stop();

            _logger.LogInformation($"stemming algorithm took {sw.Elapsed.TotalMilliseconds * 1000} µs (microseconds)");

            return stems.AsEnumerable();
        }

        public void Summarize(string Content)
        {
            
        }

        public IEnumerable<string> ToSentences(string Content)
        {
            var nonWords = new Regex("[^a-zA-Z]");
            var sentences = Content.Split(". ", StringSplitOptions.RemoveEmptyEntries)
                .Except(_models.DefaultStopWords, StringComparer.OrdinalIgnoreCase)
                .AsEnumerable<string>();
            var newSentences = new List<string>();

            foreach (var s in sentences)
            {
                var newS = s;
                foreach (var ss in newS.Split(" "))
                {
                    if (nonWords.IsMatch(ss))
                        newS = newS.Replace(ss, " ");
                }

                newSentences.Add(newS);
            }

            return newSentences;
        }
    }
}
