﻿
using System;

namespace Model.Natives
{
    public class Constant : Parameter, ICloneable
    {
        public int ParamType = 2; // DO NOT CHANGE
        public string codeText;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}