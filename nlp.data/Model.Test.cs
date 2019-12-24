using System;
using System.Collections.Generic;
using System.Text;

namespace nlp.data
{
    public class TestModel : IModel<TestModel>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public string TestMethod => "Test";
        public Guid PublicKey => Guid.NewGuid();
        public ICollection<TestModel> Children { get; set; } = new List<TestModel>();
    }
}
