using System;
using System.Collections.Generic;
using System.Text;

namespace nlp.services.text
{
    public interface ISummarizer
    {
        public IEnumerable<string> ToSentences(string Content, IEnumerable<string> StopWords = null);
        public double[,] BuildSimilarityMatrix(IEnumerable<string> Sentences, IEnumerable<string> StopWords = null);
        public double SentenceSimilarity(string Sentence1, string Sentence2, IEnumerable<string> StopWords = null);
        public double CosineDistance(int[] Vector1, int[] Vector2);
    }
}
