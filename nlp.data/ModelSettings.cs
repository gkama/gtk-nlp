using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace nlp.data
{
    public class ModelSettings<T> : IModelSettings<T>
        where T : IModel<T>
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("model")]
        public T Model { get; set; }

        [JsonPropertyName("stopWords")]
        public string[] StopWords { get; set; }

        [JsonPropertyName("stopWordsLength")]
        public int? StopWordsLength => StopWords?.Sum(x => x.Length);

        [JsonPropertyName("delimiters")]
        public string[] Delimiters { get; set; }

        [JsonPropertyName("publickey")]
        public Guid PublicKey => Guid.NewGuid();
    }
}
