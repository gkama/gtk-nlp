using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace nlp.data
{
    public class Model : IModel<Model>
    {
        public string id { get; set; }
        public string name { get; set; }
        public string details { get; set; }

        [JsonIgnore]
        public IEnumerable<string> details_split => details?.Split('|');

        public ICollection<Model> children { get; set; } = new List<Model>();
    }
}
