using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.Extensions.Logging;

using nlp.data;
using nlp.data.text;

namespace nlp.services.text
{
    public class TextMiningRepository<T> : ITextMiningRepository<T>
        where T : IModel<T>, new()
    {
        private readonly ILogger<TextMiningRepository<T>> _logger;
        private readonly IStemmer _stemmer;
        private readonly ISummarizer _summarizer;
        private readonly Models<T> _models;

        public TextMiningRepository(ILogger<TextMiningRepository<T>> logger, IStemmer stemmer, ISummarizer summarizer, Models<T> models)
        {
            _logger = logger ?? throw new NlpException(HttpStatusCode.InternalServerError, nameof(logger));
            _stemmer = stemmer ?? throw new NlpException(HttpStatusCode.InternalServerError, nameof(stemmer));
            _summarizer = summarizer ?? throw new NlpException(HttpStatusCode.InternalServerError, nameof(summarizer));
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

        public IEnumerable<IStemmedWord> Stem(ITextRequest Request)
        {
            var sw = new Stopwatch();
            var stems = new List<IStemmedWord>();

            sw.Start();
            Request.Content.Split(_models.DefaultDelimiters)
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

        public string Summarize(ITextRequest Request)
        {
            if (Request.NumberOfSentences == 0
                && Request.StopWords == null)
                return _summarizer.Summarize(Request.Content);
            else if (Request.NumberOfSentences == 0)
                return _summarizer.Summarize(Request.Content, 5, Request.StopWords);
            else
                return _summarizer.Summarize(Request.Content, Request.NumberOfSentences, Request.StopWords);
        }
        public string Summarize(string Content)
        {
            return _summarizer.Summarize(Content);
        }
        public IEnumerable<string> ToSentences(string Content, IEnumerable<string> StopWords = null)
        {
            return _summarizer.ToSentences(Content, StopWords);
        }
    }
}
