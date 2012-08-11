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
        Boolean Connect
        {
            get;
            set;
        }
        int Auth();
        int Auth(bool listen);
        int Auth(bool listen, bool enrollment);
        IMessage Send(IMessage message);
        IMessage Receive();
    }
    public class StubConnection
    {
        private Boolean _zombie;
        public Boolean Zombie
        {
            get { return _zombie; }
            set { _zombie = value; }
        }
        
        private UInt16 _pingcount;
        public UInt16 PingCount
        {
            get { return _pingcount; }
            set { _pingcount = value; }
        }
        
        private Dictionary<UInt16, Time> _OutstandingPings;
        public Dictionary<UInt16, Time> OutstandingPings
        {
            get { return _OutstandingPings; }
            set { _OutstandingPings = value; }
        }
        
        private Socket _socket;
        public Socket Socket
        {
            get
            {
                return _socket;
            }
            set
            {
                _socket = value;
            }
        }

        private Connector _connector;
        public Connector Connector
        {
            get
            {
                return _connector;
            }
            set
            {
                _connector = value;
            }
        }

        private TypeList _typelist;
        public TypeList TypeList
        {
            get
            {
                return _typelist;
            }
            set
            {
                _typelist = value;
            }
        }

        private Node _node;
        public Node Node
        {
            get
            {
                return _node;
            }
            set
            {
                _node = value;
            }
        }

    }
}
