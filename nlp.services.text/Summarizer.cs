using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Logging;

using nlp.data;

namespace nlp.services.text
{
    public class Summarizer : ISummarizer
    {
        private readonly ILogger<Summarizer> _logger;

        public Summarizer(ILogger<Summarizer> logger)
        {
            _logger = logger ?? throw new NlpException(HttpStatusCode.InternalServerError, nameof(logger));
        }

        public void Summarize(string Content, int N = 5, IEnumerable<string> StopWords = null)
        {
            var sentences = ToSentences(Content, StopWords);
            var similarityMatrix = BuildSimilarityMatrix(sentences, StopWords);
            var scores = PageRank(similarityMatrix);

            var k = 1;
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

        public IEnumerable<double> PageRank(double[,] Matrix, double eps = 0.0001, double d = 0.85)
        {
            var n = Matrix.GetLength(0);
            var ones = new int[n].Populate(1);
            var pp = new List<double>();
            var tor = new List<List<double>>();

            var m = Enumerable.Range(0, Matrix.GetLength(0))
                .Select(x => Matrix[1, x])
                .ToArray();

            foreach (var o in ones)
            {
                pp.Add(o / n);
            }

            while (true)
            {
                var newP = new List<double>();
                var numerator = new List<double>();
                var denominator = new List<double>();

                foreach (var o in ones)
                    numerator.Add(o * (1 - d));
                foreach (var p in pp)
                    denominator.Add(n + (d * m.ToArray().DotProduct(pp.ToArray())));
                newP = numerator.Zip(denominator, (a, b) => a / b).ToList();

                var delta = Math.Abs(newP.Minus(pp).Sum());

                if (delta <= eps)
                    return newP;

                pp = newP;
            }
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
