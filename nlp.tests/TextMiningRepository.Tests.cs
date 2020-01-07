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
    public class TextMiningRepositoryTests
    {
        private readonly ILogger<TextMiningRepository<Model>> _logger;
        private readonly IStemmer _stemmer;
        private readonly Models<Model> _models;
        private readonly ITextMiningRepository<Model> _repo;

        public TextMiningRepositoryTests()
        {
            _logger = Mock.Of<ILogger<TextMiningRepository<Model>>>();
            _stemmer = new Stemmer();
            _models = new Models<Model>();
            _repo = new TextMiningRepository<Model>(_logger, _stemmer, _models);
        }

        [Theory]
        [InlineData("One sentence. Two sentences. And a 3.")]
        public void ToSentences_Valid(string Content)
        {
            var sentences = _repo.ToSentences(Content)
                .ToArray();

            if (Content == "One sentence. Two sentences. And a 3.")
            {
                Assert.Equal("One sentence", sentences[0]);
                Assert.Equal("Two sentences", sentences[1]);
                Assert.Equal("And a  ", sentences[2]);
            }
        }

        [Theory]
        [InlineData("Julie loves me more than Linda loves me", "Jane likes me more than Julie loves me")]
        [InlineData("This is another test", "Let's test another similarity")]
        public void SentenceSimilarity_Similar(string Sentence1, string Sentence2)
        {
            var similarity = _repo.SentenceSimilarity(Sentence1, Sentence2);

            Assert.True(similarity > 0);
        }

        [Theory]
        [InlineData(1, 2, 0, 1, 0, 0)]
        [InlineData(1, 1, 1, 0, 0, 0, 0, 1)]
        [InlineData(1, 2, 3, 1, 2, 3)]
        [InlineData(1, 1, 1, 1, 1, 1)]
        public void CosineDistance_Valid(params int[] Vector)
        {
            var vector1 = Vector.Take(Vector.Length / 2).ToArray();
            var vector2 = Vector.Skip(Vector.Length / 2).ToArray();

            var cosineDistance = _repo.CosineDistance(vector1, vector2);

            Assert.True(cosineDistance > 0);
        }
    }
}
