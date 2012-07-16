using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace SimpleMesh.Service
{
    public class MessageBrokerArgs
    {
        public Connector Connector;
        public Socket Socket;
    }
}
