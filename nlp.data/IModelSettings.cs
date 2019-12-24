using System;
using System.Text.Json.Serialization;

namespace nlp.data
{
    public interface IModelSettings<T>
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("model")]
        public T Model { get; set; }

        [JsonPropertyName("stopWords")]
        public string[] StopWords { get; set; }

        [JsonPropertyName("stopWordsLength")]
        public int? StopWordsLength { get; }

        [JsonPropertyName("delimiters")]
        public char[] Delimiters { get; set; }

        [JsonPropertyName("publickey")]
        public Guid PublicKey { get; }
    }
}
