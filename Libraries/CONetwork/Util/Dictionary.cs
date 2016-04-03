﻿using System.Collections.Generic;

namespace CONetwork.Util
{
    public static partial class Extentions
    {
        public static Tvalue TryGet<Tkey, Tvalue>(this Dictionary<Tkey, Tvalue> dic, Tkey value)
        {
            Tvalue Out;
            return dic.TryGetValue(value, out Out) ? Out : default(Tvalue);
        }
    }
}