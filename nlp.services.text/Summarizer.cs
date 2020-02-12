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
            foreach (var s in scores.OrderByDescending(x => x).Take(N))
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

            _sw.Restart();
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
            _sw.Stop();

            _logger.LogInformation($"tosentences algorithm took {_sw.Elapsed.TotalMilliseconds * 1000} µs (microseconds)");

            return newSentences;
        }

        public IEnumerable<double> PageRank(double[,] Matrix)
        {
            var n = Matrix.GetLength(0);
            var ones = new double[n].Populate(1.0);
            var rank = new List<double>();

            for (int i = 0; i < n; i++)
            {
                var m = Enumerable.Range(0, Matrix.GetLength(1))
                    .Select(x => Matrix[i, x])
                    .ToArray();

                rank.Add(0.15 + (0.85 * (m.Zip(ones, (d1, d2) => d1 * d2).Sum())));
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
            if (string.IsNullOrWhiteSpace(Sentence1))
                throw new NlpException(HttpStatusCode.BadRequest, $"'sentence1' is not a sentence");

            if (string.IsNullOrWhiteSpace(Sentence2))
                throw new NlpException(HttpStatusCode.BadRequest, $"'sentence2' is not a sentence");

            _sw.Restart();

            var s1Words = Sentence1.Split(" ")
                .Select(x => x.ToLower());

            var s2Words = Sentence2.Split(" ")
                .Select(x => x.ToLower());

            var allWords = s1Words.Concat(s2Words)
                .ToList();

            var vector1 = new int[allWords.Count()];
            var vector2 = new int[allWords.Count()];

            foreach (var w in s1Words)
            {
                if (StopWords?.Contains(w) == true)
                    continue;

                vector1[allWords.IndexOf(w)] += 1;
            }

            foreach (var w in s2Words)
            {
                if (StopWords?.Contains(w) == true)
                    continue;

                vector2[allWords.IndexOf(w)] += 1;
            }
            _sw.Stop();

            _logger.LogInformation($"sentence similarity algorithm took {_sw.Elapsed.TotalMilliseconds * 1000} µs (microseconds)");

            return 1 - CosineDistance(vector1, vector2);
        }

        public double SentenceSimilarity2(string Sentence1, string Sentence2, IEnumerable<string> StopWords = null)
        {
            _sw.Restart();

            var s1Words = Sentence1.Split(" ")
                .Select(x => x.ToLower())
                .ToArray();

            var s2Words = Sentence2.Split(" ")
                .Select(x => x.ToLower())
                .ToArray();

            var allWords = s1Words.Concat(s2Words)
                .ToList();

            var itWords = s1Words.Count() >= s2Words.Count()
                ? s2Words
                : s1Words;

            var v = new double[itWords.Count()];

            for (int i = 0; i < itWords.Count(); i++)
            {
                var s1w = s1Words[i];
                var s2w = s2Words[i];

                if (StopWords?.Contains(s1w, StringComparer.OrdinalIgnoreCase) == true
                    || StopWords?.Contains(s2w, StringComparer.OrdinalIgnoreCase) == true)
                    continue;

                v[allWords.IndexOf(s1w)] = WordSimilarity(s1w, s2w);
            }
            _sw.Stop();

            _logger.LogInformation($"sentence similarity 2 algorithm took {_sw.Elapsed.TotalMilliseconds * 1000} µs (microseconds)");

            return v.Average();
        }

        public double WordSimilarity(string Word1, string Word2)
        {
            var w1Chars = Word1.Contains(" ")
                ? Word1.Trim().ToCharArray()
                : Word1.ToCharArray();

            var w2Chars = Word2.Contains(" ")
                ? Word2.Trim().ToCharArray()
                : Word2.ToCharArray();

            var allChars = w1Chars.Concat(w2Chars)
                .ToList();

            var v1 = new int[allChars.Count()];
            var v2 = new int[allChars.Count()];

            foreach (var c in w1Chars)
                v1[allChars.IndexOf(c)] += 1;
            foreach (var c in w2Chars)
                v2[allChars.IndexOf(c)] += 1;

            return 1 - CosineDistance(v1, v2);
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

        public object Sample()
        {
            var contentText = "In an attempt to build an AI-ready workforce, Microsoft announced Intelligent Cloud Hub which has been launched to empower the next generation of students with AI-ready skills. Envisioned as a three-year collaborative program, Intelligent Cloud Hub will support around 100 institutions with AI infrastructure, course content and curriculum, developer support, development tools and give students access to cloud and AI services. As part of the program, the Redmond giant which wants to expand its reach and is planning to build a strong developer ecosystem in India with the program will set up the core AI infrastructure and IoT Hub for the selected campuses. The company will provide AI development tools and Azure AI services such as Microsoft Cognitive Services, Bot Services and Azure Machine Learning.According to Manish Prakash, Country General Manager-PS, Health and Education, Microsoft India, said, 'With AI being the defining technology of our time, it is transforming lives and industry and the jobs of tomorrow will require a different skillset.This will require more collaborations and training and working with AI.That’s why it has become more critical than ever for educational institutions to integrate new cloud and AI technologies.The program is an attempt to ramp up the institutional set - up and build capabilities among the educators to educate the workforce of tomorrow.' The program aims to build up the cognitive skills and in-depth understanding of developing intelligent cloud connected solutions for applications across industry. Earlier in April this year, the company announced Microsoft Professional Program In AI as a learning track open to the public. The program was developed to provide job ready skills to programmers who wanted to hone their skills in AI and data science with a series of online courses which featured hands-on labs and expert instructors as well. This program also included developer-focused AI school that provided a bunch of assets to help build AI skills.";

            return new
            {
                text = contentText,
                numerOfSentences = 3,
                summary = Summarize(contentText, 3)
            };
        }
    }
}
