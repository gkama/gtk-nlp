using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Xunit;
using Moq;

using nlp.data;
using nlp.services;

namespace nlp.tests
{
    public class NlpRepositoryModelTests
    {
        private readonly ILogger<NlpRepository<Model>> _mockLogger;
        private readonly INlpRepository<Model> _repo;

        public NlpRepositoryModelTests()
        {
            _mockLogger = Mock.Of<ILogger<NlpRepository<Model>>>();
            _repo = new NlpRepository<Model>(_mockLogger, new Models<Model>());
        }

        [Theory]
        [InlineData("1")]
        [InlineData("2")]
        [InlineData("3")]
        public void GetModel_EqualsNull(string id)
        {
            var model = _repo.GetModel(id);

            //Asserts
            Assert.Null(model);
        }
    }
}
