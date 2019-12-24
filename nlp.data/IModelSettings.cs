using System;
using System.Collections.Generic;
using System.Text;

namespace nlp.data
{
    public interface IModelSettings<T>
    {
        public string Id { get; set; }
        public string ModelId { get; set; }
        public string[] StopWords { get; set; }
        public string[] Delimiters { get; set; }
        public int? StopWordsLength { get; }
        public Guid PublicKey { get; }
    }
}
