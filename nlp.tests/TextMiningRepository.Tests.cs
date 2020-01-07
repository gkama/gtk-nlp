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
                Assert.Equal("And a ", sentences[2]);
            }
        }
    }
}
