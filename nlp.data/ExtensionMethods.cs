using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace nlp.data
{
    public static class ExtensionMethods
    {
        public static T ToObject<T>(this JsonElement element)
        {
            return JsonSerializer.Deserialize<T>(element.GetRawText());
        }
    }
}
