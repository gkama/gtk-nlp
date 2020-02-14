using System;
using System.Collections.Generic;
using System.Text;

using nlp.data.text;

namespace nlp.services.text
{
    public interface IStemmer
    {
        public IStemmedWord Stem(string Word);
        public object Sample();
    }
}
