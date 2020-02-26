using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

using Xunit;
using Moq;

using nlp.data;
using nlp.services;
using nlp.services.text;

namespace nlp.tests
{
    public class NlpRepositoryModelTests
    {
        private readonly INlpRepository<Model> _repo;

        public NlpRepositoryModelTests()
        {
            _repo = new Helper<INlpRepository<Model>>().GetService();
        }

        [Theory]
        [InlineData("this is to try and tokenize this.")]
        [InlineData("testing, testing, testing")]
        [InlineData("aaaaaaaaaaaaa, b. c. d. Also this is a sentence.")]
        public void Tokenize_Valid(string Content)
        {
            var tokenized = _repo.Tokenize(Content, new char[] { ',', '.' }, new string[] { "stop", "me" });

            Assert.NotEmpty(tokenized);
        }

        [Fact]
        public void GetNlpRequestSchema_Valid()
        {
            var schema = _repo.GetNlpRequestSchema();

            Assert.NotNull(schema);
        }

        [Fact]
        public void Sample_Valid()
        {
            var sample = _repo.Sample();

            Assert.NotNull(sample);
        }
    }
}
