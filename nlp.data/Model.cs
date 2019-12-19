using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace nlp.data
{
    public class Model : IModel<Model>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public Guid PublicKey => Guid.NewGuid();

        [JsonIgnore]
        public IEnumerable<string> DetailsSplit => Details?.Split('|');

        public ICollection<Model> Children { get; set; } = new List<Model>();
    }
}
