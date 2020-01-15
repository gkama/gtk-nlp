using System;
using System.Collections.Generic;
using System.Linq;
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
        public void ToObjectT_Valid(string id)
        {
            var jsonStr = "{ \"id\": \"{id}\" }".Replace("{id}", id);
            var json = JsonDocument.Parse(jsonStr).RootElement;

            Assert.Equal(id, json.GetProperty("id").ToObject<string>());
        }

        [Theory]
        [InlineData("1")]
        [InlineData("2")]
        [InlineData("3")]
        [InlineData("4")]
        [InlineData("5")]
        public void DeserializeSelfReferencing_Valid(string id)
        {
            var jsonStr = "{ \"id\": \"{id}\", \"name\": \"test\", \"details\": \"test,1,2,3\", \"children\": [] }".Replace("{id}", id);
            var json = JsonDocument.Parse(jsonStr).RootElement;

            Assert.Equal(id, json.GetRawText().DeserializeSelfReferencing<Model>().Id);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData("one")]
        [InlineData("two")]
        [InlineData("three")]
        public void PopulateArray_Valid(object value)
        {
            var arr = new object[5].Populate(value);

            Assert.All(arr, v =>
            {
                Assert.Equal(v, value);
            });
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(0, 1)]
        [InlineData(1, 1)]
        [InlineData(2, 3)]
        [InlineData(2, 2)]
        [InlineData(3, 3)]
        public void DotProduct_Valid(int pos1, int pos2)
        {
            var dotProduct = ArrayTestData.ToArray()[pos1]
                .DotProduct(ArrayTestData.ToArray()[pos2]);

            Assert.True(dotProduct >= 1);
        }

        public static IEnumerable<double[]> ArrayTestData =>
            new[]
            {
                new []{ 1.0, 1.0 },
                new []{ 2.0, 2.0 },
                new []{ 3.0, 2.0 },
                new []{ 5.0, 0.0 }
            };
    }
}
