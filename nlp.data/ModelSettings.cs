using System;
using System.Collections.Generic;
using System.Linq;

namespace nlp.data
{
    public class ModelSettings<T> : IModelSettings<T>
        where T : IModel<T>
    {
        public string Id { get; set; }
        public T Model { get; set; }     
        public string[] StopWords { get; set; }
        public int? StopWordsLength => StopWords?.Sum(x => x.Length);
        public string[] Delimiters { get; set; }
        public Guid PublicKey => Guid.NewGuid();
    }
}
