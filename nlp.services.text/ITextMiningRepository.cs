using System;
using System.Collections.Generic;
using System.Text;

using nlp.data;

namespace nlp.services.text
{
    public interface ITextMiningRepository<T>
    {
        public object Mine(string Content);
        public IEnumerable<IStemmedWord> Stem(string Content);
        public string Summarize(string Content, int N = 5, IEnumerable<string> StopWords = null);
    }
}
