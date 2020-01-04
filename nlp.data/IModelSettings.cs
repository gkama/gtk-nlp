using System;
using System.Text.Json.Serialization;

namespace nlp.data
{
    public interface IModelSettings<T>
    {
        public string Id { get; set; }
        public T Model { get; set; }
        public string[] StopWords { get; set; }
        public int? StopWordsLength { get; }
        public char[] Delimiters { get; set; }
        public Guid PublicKey { get; }
    }
}
