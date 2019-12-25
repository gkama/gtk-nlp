using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace nlp.data
{
    public interface IModel<T>
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("details")]
        public string Details { get; set; }

        [JsonPropertyName("publickey")]
        public Guid PublicKey { get; }

        [JsonPropertyName("children")]
        public ICollection<T> Children { get; set; }
    }
}
