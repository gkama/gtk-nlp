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
    public class NlpRepositoryTests
    {
        private readonly ILogger<NlpRepository<Model>> _mockLogger;
        private readonly INlpRepository<Model> _repo;

        public NlpRepositoryTests()
        {
            _mockLogger = Mock.Of<ILogger<NlpRepository<Model>>>();
            _repo = new NlpRepository<Model>(_mockLogger);
        }
    }
}
