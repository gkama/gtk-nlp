using System;
using System.Collections.Generic;
using System.Text;

using nlp.data;

namespace nlp.services
{
    public interface ITextMiningRepository<T>
    {
        public object Mine(string Content);
        public IEnumerable<IStemmedWord> Stem(string Content);
    }
}
