using System;
using System.Collections.Generic;
using System.Text;

using Xunit;
using Moq;

using Microsoft.Extensions.Logging;

using nlp.data;
using nlp.services.text;

namespace nlp.tests
{
    public class StemmerTests
    {
        private readonly ILogger<Stemmer> _logger;
        private readonly IStemmer _stemmer;

        public StemmerTests()
        {
            _logger = Mock.Of<ILogger<Stemmer>>();
            _stemmer = new Stemmer(_logger);
        }

        [Theory]
        [InlineData("testing")]
        [InlineData("word")]
        [InlineData("serialization")]
        public void Stem_Valid(string Word)
        {
            var stem = _stemmer.Stem(Word);

            Assert.NotNull(stem);
        }

        [Fact]
        public void Categorization_Categori()
        {
            var stem = _stemmer.Stem("categorization");

            Assert.Equal("categor", stem.Value);
        }
    }
}
