using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Xunit;
using Moq;

using Microsoft.Extensions.Logging;

using nlp.data;
using nlp.services.text;

namespace nlp.tests
{
    public class SummarizerTests
    {
        private readonly ISummarizer _summarizer;

        public SummarizerTests()
        {
            _summarizer = new Summarizer();
        }

        [Theory]
        [InlineData("One sentence. Two sentences. And a 3.")]
        public void ToSentences_Valid(string Content)
        {
            var sentences = _summarizer.ToSentences(Content)
                .ToArray();

            if (Content == "One sentence. Two sentences. And a 3.")
            {
                Assert.Equal("One sentence", sentences[0]);
                Assert.Equal("Two sentences", sentences[1]);
                Assert.Equal("And a  ", sentences[2]);
            }
        }

        [Theory]
        [InlineData("Julie loves me more than Linda loves me", "Jane likes me more than Julie loves me", "This is another test", "Let's test another similarity")]
        [InlineData("Test me", "Test me as well", "Test me as well please", "What about me?")]
        public void BuildSimilarityMatrix_Similar(params string[] Sentences)
        {
            var matrix = _summarizer.BuildSimilarityMatrix(Sentences);

            Assert.NotNull(matrix);
            Assert.True(matrix[0, 1] > 0);
            Assert.True(matrix[1, 0] > 0);
        }

        [Theory]
        [InlineData("Julie loves me more than Linda loves me", "Jane likes me more than Julie loves me")]
        [InlineData("This is another test", "Let's test another similarity")]
        public void SentenceSimilarity_Similar(string Sentence1, string Sentence2)
        {
            var similarity = _summarizer.SentenceSimilarity(Sentence1, Sentence2);

            Assert.True(similarity > 0);
        }

        [Theory]
        [InlineData(1, 2, 0, 1, 0, 0)]
        [InlineData(1, 1, 1, 0, 0, 0, 0, 1)]
        [InlineData(1, 2, 3, 1, 2, 3)]
        [InlineData(1, 1, 1, 1, 1, 2)]
        public void CosineDistance_BiggerThanZero(params int[] Vector)
        {
            var vector1 = Vector.Take(Vector.Length / 2).ToArray();
            var vector2 = Vector.Skip(Vector.Length / 2).ToArray();

            var cosineDistance = _summarizer.CosineDistance(vector1, vector2);

            Assert.True(cosineDistance >= 0);
        }
    }
}
