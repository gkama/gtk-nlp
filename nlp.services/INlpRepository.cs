using System;
using System.Collections.Generic;
using System.Text;

using nlp.data;

namespace nlp.services
{
    public interface INlpRepository<T>
    {
        string Categorize();
    }
}
