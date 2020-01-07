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

        public double[,] BuildSimilarityMatrix(IEnumerable<string> Sentences, IEnumerable<string> StopWords = null)
        {
            var similarityMatrix = new double[Sentences.Count(), Sentences.Count()];
            var sentencesArray = Sentences.ToArray();

            for (int idx1 = 0; idx1 < Sentences.Count(); idx1++)
            {
                for (int idx2 = 0; idx2 < Sentences.Count(); idx2++)
                {
                    if (idx1 == idx2)
                        continue;

                    similarityMatrix[idx1, idx2] = SentenceSimilarity(sentencesArray[idx1], sentencesArray[idx2], StopWords);
                }
            }

            return similarityMatrix;
        }

        public double SentenceSimilarity(string Sentence1, string Sentence2, IEnumerable<string> StopWords = null)
        {
            if (string.IsNullOrWhiteSpace(Sentence1)
                || Sentence1.Contains(".")) throw new NlpException(HttpStatusCode.BadRequest, $"'sentence1' is not a sentence");

            if (string.IsNullOrWhiteSpace(Sentence2)
                || Sentence2.Contains(".")) throw new NlpException(HttpStatusCode.BadRequest, $"'sentence2' is not a sentence");

            var s1Words = Sentence1.Split(" ")
                .Select(x => x.ToLower());

            var s2Words = Sentence2.Split(" ")
                .Select(x => x.ToLower());

            var allWords = s1Words.Concat(s2Words);

            var vector1 = new int[allWords.Count()];
            var vector2 = new int[allWords.Count()];

            foreach (var w in s1Words)
            {
                if (StopWords?.Contains(w) == true)
                    continue;

                vector1[allWords.ToList().IndexOf(w)] += 1;
            }

            foreach (var w in s2Words)
            {
                if (StopWords?.Contains(w) == true)
                    continue;

                vector2[allWords.ToList().IndexOf(w)] += 1;
            }

            return 1 - CosineDistance(vector1, vector2);
        }

        public double CosineDistance(int[] Vector1, int[] Vector2)
        {
            var dotProduct = 0.0;
            var normA = 0.0;
            var normB = 0.0;

            for (int i = 0; i < Vector1.Length; i++)
            {
                dotProduct += Vector1[i] * Vector2[i];
                normA += Math.Pow(Vector1[i], 2);
                normB += Math.Pow(Vector2[i], 2);
            }

            return 1 - (dotProduct / (Math.Sqrt(normA) * Math.Sqrt(normB)));
        }
    }
}
