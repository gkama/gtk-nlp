using System;
using System.Collections.Generic;
using System.Linq;

namespace nlp.data
{
    public class ModelSettings
    {
        public string[] StopWords { get; set; }
        public char[] Delimiters { get; set; }

        public int? StopWordsLength => StopWords?.Sum(x => x.Length);
    }
}
