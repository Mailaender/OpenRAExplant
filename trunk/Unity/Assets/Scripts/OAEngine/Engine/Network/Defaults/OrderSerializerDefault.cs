﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Engine.Network.Interfaces;

namespace Engine.Network.Defaults
{
    public class OrderSerializerDefault : IOrderSerializer
    {
        public List<IOrder> Deserialize(INetWorld world, byte[] bytes)
        {
            return bytes.ToOrderList(world);
        }

        public IOrder Deserialize(INetWorld world, BinaryReader r)
        {
            return null;
        }

        public byte[] Serialize(IOrder order)
        {
            return null;
        }
    }
}
