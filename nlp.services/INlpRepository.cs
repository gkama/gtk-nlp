﻿using System;
using System.Collections.Generic;
using System.Text.Json;

using nlp.data;

namespace nlp.services
{
    public interface INlpRepository<T>
    {
        string Categorize(dynamic Request);
        bool Parse(dynamic Request);
    }
}
