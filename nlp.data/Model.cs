using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace nlp.data
{
    public class Model : IModel<Model>
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("details")]
        public string Details { get; set; }

        public Guid PublicKey => Guid.NewGuid();

        [JsonIgnore]
        public IEnumerable<string> DetailsSplit => Details?.Split('|');

        [JsonPropertyName("children")]
        public ICollection<Model> Children { get; set; } = new List<Model>();
    }
}
