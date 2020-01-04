﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace nlp.data
{
    public class Model : IModel<Model>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Details { get; set; }
        public Guid PublicKey => Guid.NewGuid();
        public ICollection<Model> Children { get; set; } = new List<Model>();
    }
}
