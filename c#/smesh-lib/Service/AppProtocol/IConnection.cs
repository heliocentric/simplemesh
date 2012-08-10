using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace SimpleMesh.Service.AppProtocol
{
    public interface IConnection
    {
        Boolean Zombie
        {
            get;
            set;
        }
        UInt16 PingCount
        {
            get;
            set;
        }
        Dictionary<UInt16, Time> OutstandingPings
        {
            get;
            set;
        }
        Socket Socket
        {
            get;
            set;
        }
        Connector Connector
        {
            get;
            set;
        }
        TypeList TypeList
        {
            get;
            set;
        }
        Node Node
        {
            get;
            set;
        }


    }
    public class StubConnection
    {

    }
}
