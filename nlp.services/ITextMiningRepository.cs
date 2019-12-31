using System;
using System.Collections.Generic;
using System.Text;

namespace nlp.services
{
    public interface ITextMiningRepository<T>
    {
        public object Mine(string Content);
        public object Stem(string Content);
    }
}
