﻿using System;

namespace ArnoldVinkCode
{
    public partial class AVClasses
    {
        [Serializable]
        public class ProfileShared
        {
            public string String1 { get; set; }
            public string String2 { get; set; }
            public string String3 { get; set; }
            public int? Int1 { get; set; }
            public object Object1 { get; set; }
        }
    }
}