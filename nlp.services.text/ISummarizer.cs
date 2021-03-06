﻿using System;
using System.Collections.Generic;
using System.Text;

namespace nlp.services.text
{
    public interface ISummarizer
    {
        public string Summarize(string Content, int N = 5, IEnumerable<string> StopWords = null);
        public IEnumerable<string> ToSentences(string Content, IEnumerable<string> StopWords = null);
        public IEnumerable<double> PageRank(double[,] Matrix);
        public double[,] BuildSimilarityMatrix(IEnumerable<string> Sentences, IEnumerable<string> StopWords = null);
        public double SentenceSimilarity(string Sentence1, string Sentence2, IEnumerable<string> StopWords = null);
        public double SentenceSimilarity2(string Sentence1, string Sentence2, IEnumerable<string> StopWords = null);
        public double WordSimilarity(string Word1, string Word2);
        public double CosineDistance(int[] Vector1, int[] Vector2);
        public object Sample();
    }
}
