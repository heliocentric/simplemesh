using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleMesh;

namespace MeshBroker
{
    class Program
    {
        static void Main(string[] args)
        {
            SimpleMesh.Service.Runner Main = new SimpleMesh.Service.Runner();
            Main.Start();
        }
    }
}
