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

        [Theory]
        [InlineData("1")]
        [InlineData("2")]
        [InlineData("3")]
        [InlineData("4")]
        [InlineData("5")]
        public void ToModelT_Valid(string id)
        {
            var jsonStr = "{ \"id\": \"{id}\", \"name\": \"test\", \"details\": \"test,1,2,3\", \"children\": [] }".Replace("{id}", id);
            var json = JsonDocument.Parse(jsonStr).RootElement;

            //Asserts
            Assert.True(json.ToModel<Model>().Id == id);
        }
    }
}
