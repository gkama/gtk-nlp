using System;
using System.Collections.Generic;
using System.Text;

namespace nlp.data
{
    public class Model : IModel<Model>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public Guid PublicKey { get; set; } = Guid.NewGuid();
        public ICollection<Model> Children { get; set; } = new List<Model>();
    }
}
