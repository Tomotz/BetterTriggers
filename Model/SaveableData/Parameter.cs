﻿using Model.JsonBaseConverter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Model.SaveableData
{
    [JsonConverter(typeof(BaseConverter))]
    public class Parameter
    {
        public string identifier; // For convenience, values also use this field.
        public string returnType;
    }
}
