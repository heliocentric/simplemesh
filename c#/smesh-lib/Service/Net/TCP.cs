using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace SimpleMesh.Service.Net
{
    public class TCPConnector : IConnection
    {
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
                return this._node;
            }
            set
            {
                this._node = value;
            }
        }
        public int Send(Message message)
        {
            int retval = 1;
            return retval;
        }
        public Message Recieve()
        {
            return new Message();
        }
    }
}
