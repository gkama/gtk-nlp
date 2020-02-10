using System;
using System.Collections.Generic;
using System.Text.Json;

using nlp.data;

namespace nlp.services
{
    public interface INlpRepository<T>
    {
        public INlpResponse Categorize(INlpRequest<T> Request, string Id = null, bool Summarize = false);
        public IModelSettings<T> Parse(INlpRequest<T> Request, string Id = null);
        public object CategorizeSample();
        public object GetNlpRequestSchema();
    }
}
