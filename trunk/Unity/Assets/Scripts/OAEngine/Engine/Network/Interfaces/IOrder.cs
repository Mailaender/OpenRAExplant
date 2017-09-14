﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Engine.Network.Interfaces
{
    public interface IOrder
    {

        bool IsImmediate { set;get; }

        string OrderString { get; }

        byte[] ExtDatas { get; }

        byte[] Serialize();



        //void Deserialize(IWorld world, BinaryReader r);
    }
}
