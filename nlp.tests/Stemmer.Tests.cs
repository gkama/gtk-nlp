using System;
using System.Collections.Generic;
using System.Text;

using Xunit;
using Moq;

using nlp.data;
using nlp.services.text;

namespace nlp.tests
{
    public class StemmerTests
    {
        private readonly IStemmer _stemmer;

        public StemmerTests()
        {
            _stemmer = new Stemmer();
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
