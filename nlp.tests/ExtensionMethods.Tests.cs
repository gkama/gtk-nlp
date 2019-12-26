using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Xunit;
using Moq;

using nlp.data;
using nlp.services;

namespace nlp.tests
{
    public class ExtensionMethodsTests
    {
        public ExtensionMethodsTests()
        {

        }

        [Fact]
        public void ToObjectT_Valid()
        {
            var jsonStr = "{ \"id\": \"1\", \"name\": \"test\", \"details\": \"test,1,2,3\", \"children\": [] }";
            var json = JsonDocument.Parse(jsonStr).RootElement;

            //Asserts
            Assert.NotNull(json.ToObject<Model>());
        }
    }
}
