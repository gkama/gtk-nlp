using System;
using System.Collections.Generic;
using System.Text;

using nlp.data;

namespace nlp.services.text
{
    public interface ITextMiningRepository<T>
    {
        public object Mine(string Content);
        public IEnumerable<IStemmedWord> Stem(string Content);
        public IEnumerable<string> ToSentences(string Content);
        public double[,] BuildSimilarityMatrix(IEnumerable<string> Sentences, IEnumerable<string> StopWords = null);
        public double SentenceSimilarity(string Sentence1, string Sentence2, IEnumerable<string> StopWords = null);
        public double CosineDistance(int[] Vector1, int[] Vector2);
    }
}
