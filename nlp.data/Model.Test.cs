using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace nlp.data
{
    public class TestModel : IModel<TestModel>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public string TestMethod => "Test";
        [JsonIgnore] public string TestMethod2 => "Test 2";
        public Guid PublicKey { get; set; } = Guid.NewGuid();
        public ICollection<TestModel> Children { get; set; } = new List<TestModel>();
    }
}
