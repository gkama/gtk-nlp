using System;
using System.Collections.Generic;
using System.Text;

namespace nlp.data
{
    public interface IModelSettings<T>
        where T : class
    {
        public string[] StopWords { get; set; }
        public char[] Delimiters { get; set; }
        public int? StopWordsLength { get; }
        public Guid PublicKey { get; }
    }
}
