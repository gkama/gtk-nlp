using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace nlp.data
{
    public interface IModel<T>
    {
        [JsonPropertyName("id")]
        string Id { get; set; }

        [JsonPropertyName("name")]
        string Name { get; set; }

        [JsonPropertyName("details")]
        string Details { get; set; }

        [JsonPropertyName("publickey")]
        public Guid PublicKey { get; }

        [JsonPropertyName("children")]
        ICollection<T> Children { get; set; }
    }
}
