using System;
using System.Collections.Generic;
using System.Text;

using nlp.data;

namespace nlp.services.text
{
    public interface IStemmer
    {
        public IStemmedWord Stem(string Word);
    }
}
