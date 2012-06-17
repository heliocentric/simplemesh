using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleMesh
{
    public static class Utility
    {
        public static  byte [] ToNetworkOrder(UInt16 source) {
            byte[] mesh1;
            byte[] mesh2;
            mesh1 = new byte[2];
            mesh1 = BitConverter.GetBytes(source);
            mesh2 = new byte[2];
            mesh2[0] = mesh1[1];
            mesh2[1] = mesh1[0];
            return mesh2;
        }
        public static byte [] ToHostOrder(UInt16 source)
        {
            return ToNetworkOrder(source);
        }
    }
}
