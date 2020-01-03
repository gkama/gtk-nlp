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

            Assert.Null(model);
        }

        [Theory]
        [InlineData("5582adee-5aa5-430c-8ef6-7797d907fa2f")]
        [InlineData("ab9f07d9-a77c-42d1-87f5-cb0c189ee9e7")]
        [InlineData("0a1dfb9f-9b38-4af2-8cb7-252899ec8304")]
        public void GetAnyModel_Valid(string id)
        {
            var model = _repo.GetAnyModel(id);

            Assert.NotNull(model);
            Assert.NotNull(model.Name);
            Assert.Equal(id, model.Id);
        }
    }
}
