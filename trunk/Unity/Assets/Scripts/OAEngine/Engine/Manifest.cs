﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public class Manifest
    {
        public string Id = "";
        public ModMetadata Metadata { set; get; }
        public string[] ServerTraits { set; get; }

        
    }
}
