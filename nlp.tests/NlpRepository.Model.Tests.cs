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

        [Fact]
        public void GetNlpRequestSchema_Valid()
        {
            var schema = _repo.GetNlpRequestSchema();

            Assert.True(schema != null);
        }
    }
}
