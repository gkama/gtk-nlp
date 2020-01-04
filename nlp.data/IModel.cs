using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace nlp.data
{
    public interface IModel<T>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public Guid PublicKey { get; }
        public ICollection<T> Children { get; set; }
    }
}
