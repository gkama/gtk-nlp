using System;
using System.Collections.Generic;
using System.Text;

namespace nlp.data
{
    public interface IModel<T>
    {
        string Id { get; set; }
        string Name { get; set; }
        string Details { get; set; }
        public Guid PublicKey { get; }
        ICollection<T> Children { get; set; }
    }
}
