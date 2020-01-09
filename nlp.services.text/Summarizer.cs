using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Logging;

using nlp.data;

namespace nlp.services.text
{
    public class Summarizer : ISummarizer
    {
        private readonly ILogger<Summarizer> _logger;

        private readonly Stopwatch _sw;

        public Summarizer(ILogger<Summarizer> logger)
        {
            _logger = logger ?? throw new NlpException(HttpStatusCode.InternalServerError, nameof(logger));
            _sw = new Stopwatch();
        }

        public string Summarize(string Content, int N = 5, IEnumerable<string> StopWords = null)
        {
            _logger.LogInformation($"summarize request received with N={N}. StopWords={StopWords?.Count()}. content={Content}");

            _sw.Start();
            var sentences = ToSentences(Content, StopWords);
            var similarityMatrix = BuildSimilarityMatrix(sentences, StopWords);
            var scores = PageRank(similarityMatrix);

            var summText = new StringBuilder();
            foreach (var s in scores.OrderBy(x => x).Take(N))
            {
                var sIdx = scores.ToList().IndexOf(s);

                summText.Append(sentences.ToArray()[sIdx]
                        .TrimEnd(' '))
                    .Append(". ");
            }
            _sw.Stop();

            _logger.LogInformation($"summarize algorithm took {_sw.Elapsed.TotalMilliseconds * 1000} µs (microseconds)");

            return summText.ToString()
                .TrimEnd(' ');
        }

        public IEnumerable<string> ToSentences(string Content, IEnumerable<string> StopWords = null)
        {
            var stopWords = StopWords == null
                ? Enumerable.Empty<string>()
                : StopWords;

            var sentences = Content.Split(". ", StringSplitOptions.RemoveEmptyEntries)
                .Except(stopWords, StringComparer.OrdinalIgnoreCase)
                .AsEnumerable<string>();

            var nonWords = new Regex("[^a-zA-Z]");
            var mSpaces = new Regex("[ ]{2,}");
            var newSentences = new List<string>();

            foreach (var s in sentences)
            {
                var newS = s;
                foreach (var ss in newS.Split(" "))
                {
                    if (nonWords.IsMatch(ss))
                        newS = newS.Replace(ss, " ");
                }

                newS = mSpaces.Replace(newS, " ");

                newSentences.Add(newS);
            }

            return newSentences;
        }

        public IEnumerable<double> PageRank(double[,] Matrix, double eps = 0.0001, double d = 0.85)
        {
            var n = Matrix.GetLength(0);
            var ones = new double[n].Populate(1.0);
            var rank = new List<double>();

            for (int i = 0; i < n; i++)
            {
                var m = Enumerable.Range(0, Matrix.GetLength(1))
                    .Select(x => Matrix[i, x])
                    .ToArray();

                rank.Add(0.15 + (d * (m.Zip(ones, (d1, d2) => d1 * d2).Sum())));
            }

            return rank.AsEnumerable();
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
                || Sentence1.Contains("."))
                throw new NlpException(HttpStatusCode.BadRequest, $"'sentence1' is not a sentence");

            if (string.IsNullOrWhiteSpace(Sentence2)
                || Sentence2.Contains("."))
                throw new NlpException(HttpStatusCode.BadRequest, $"'sentence2' is not a sentence");

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
            var norm1 = 0.0;
            var norm2 = 0.0;

            for (int i = 0; i < Vector1.Length; i++)
            {
                dotProduct += Vector1[i] * Vector2[i];
                norm1 += Math.Pow(Vector1[i], 2);
                norm2 += Math.Pow(Vector2[i], 2);
            }

            return 1 - (dotProduct / (Math.Sqrt(norm1) * Math.Sqrt(norm2)));
        }
    }
}
