/*
# Copyright 2011-2012 Dylan Cochran
# All rights reserved
#
# Redistribution and use in source and binary forms, with or without
# modification, are permitted providing that the following conditions
# are met:
# 1. Redistributions of source code must retain the above copyright
#    notice, this list of conditions and the following disclaimer.
# 2. Redistributions in binary form must reproduce the above copyright
#    notice, this list of conditions and the following disclaimer in the
#    documentation and/or other materials provided with the distribution.
#
# THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
# IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
# WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
# ARE DISCLAIMED.  IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY
# DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
# DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS
# OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
# HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
# STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING
# IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
# POSSIBILITY OF SUCH DAMAGE.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace SimpleMesh.Service.AppProtocol
{
    public interface IConnection
    {
        Boolean Vampire
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
        IMessage Receive(Boolean ignore);
        IMessage Maintenence();
        int Register(MType type);
        int Deregister(MType type);
    }
    public class StubConnection
    {
        private Boolean _vampire;
        public Boolean Vampire
        {
            get { return _vampire; }
            set { _vampire = value; }
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
