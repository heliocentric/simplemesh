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
using System.Net;

namespace SimpleMesh.Service
{
    public class Connection
    {
        public Connection()
        {
            this.TypeList = new TypeList();
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
        int Send(Message message)
        {
            return 1;
        }
        Message Recieve()
        {
            return new Message();
        }

        public int Auth()
        {
            Connection container = this;
            int retval = 99;
            TextMessage scratch;
            scratch = new TextMessage("Control.Auth.UUID");
            scratch.Data = Runner.Network.Node.UUID.ToString();
            Utility.SendMessage(container, scratch);

            List<string> authtypes = new List<string>();
            authtypes.Add("NONE");
            authtypes.Add("RSA");

            scratch = new TextMessage("Control.Auth.Types");
            string authstring = "";
            foreach (string entry in authtypes)
            {
                if (authstring == "")
                {
                    authstring = entry;
                }
                else
                {
                    authstring = authstring + " " + entry;
                }
            }
            scratch.Data = authstring;
            Utility.SendMessage(container, scratch);
            bool end;
            end = false;
            bool error = false;
            Message Recieved;
            bool uuid = false;
            bool authreceived = false;
            UUID remoteuuid = new UUID();
            TextMessage text;
            List<string> typelist;
            typelist = new List<string>();

            while (end == false)
            {
                Recieved = Utility.ReceiveMessage(container);
                if (Recieved.Type.Substring(0, 6) != "Error.")
                {
                    switch (Recieved.Type)
                    {
                        case "Control.Auth.UUID":
                            text = new TextMessage(Recieved);
                            remoteuuid = new UUID(text.Data);
                            uuid = true;
                            break;
                        case "Control.Auth.Types":
                            text = new TextMessage(Recieved);

                            string[] types = text.Data.Split(' ');
                            foreach (string type in types)
                            {
                                if (authtypes.Contains(type))
                                {
                                    typelist.Add(type);
                                }
                            }
                            authreceived = true;
                            break;
                    }
                }
                else
                {
                    end = true;
                    error = true;
                }
                if (authreceived == true && uuid == true)
                {
                    end = true;
                }
            }
            if (error == true)
            {
                retval = 1;
                return retval;
            }
            string msg;
            msg = "Auth Types available:";
            bool first = true;
            foreach (string type in typelist)
            {
                msg = msg + " " + type;
            }
            Runner.DebugMessage("Debug.Info.Connect", msg);
            string authtotry = "";
            if (typelist.Count != 0)
            {
                if (typelist.Contains("NONE"))
                {
                    authtotry = "NONE";
                }
                else
                {
                    authtotry = typelist[0];
                }
                Runner.DebugMessage("Debug.Info.Connect", "Using " + authtotry);
            }
            else
            {
                retval = 1;
                return retval;
            }
            switch (authtotry)
            {
                case "NONE":
                    Node node;
                    if (Runner.Network.NodeList.TryGetValue(remoteuuid.ToString(), out node) == true)
                    {
                        container.Node = node;
                        Runner.DebugMessage("Debug.Info.Auth", "Remote Node is: " + node.ToString());
                        retval = 0;
                    }
                    else
                    {
                        retval = 1;
                    }
                    break;
            }

            return retval;
        }
    }
    public class Connector
    {
        public TypeList Typelist;
        public Node Node;
        public enum ConnectorTypes
        {
            Listen,
            Connect
        }
        public ConnectorTypes Type;
        public int Priority;
        public string Protocol;
        public string AppProtocol;
        public string HostType;
        public string Host;
        public string TransportProtocol;
        public int Port;
        public Connector()
        {
            this.Priority = 50;
            this.Protocol = "";
            this.AppProtocol = "";
            this.Host = "";
            this.Port = 17555;
            this.HostType = "";
            this.TransportProtocol = "IP";
            this.Type = ConnectorTypes.Connect;
        }
        public Connector(string connspec)
        {
            string[] chunks = connspec.Split('!');

            try
            {
                this.Priority = Convert.ToInt32(chunks[0]);
            }
            catch
            {
                this.Priority = 50;
            }
            this.AppProtocol = chunks[1];
            this.Protocol = chunks[2];
            try
            {
                this.Port = Convert.ToInt32(chunks[3]);
            }
            catch
            {
                this.Port = 17555;
            }
            this.TransportProtocol = chunks[4];
            this.HostType = chunks[5];
            this.Host = chunks[6];
        }
        public override string ToString()
        {
            return this.ToString("1.0");
        }
        public string ToString(string version)
        {
            switch (version)
            {
                case "1.0":
                default:
                    return this.Priority + "!" + this.Key();
            }
        }
        public string Key()
        {
            return this.AppProtocol + "!" + this.Protocol + "!" + this.Port + "!" + this.TransportProtocol + "!" + this.HostType + "!" + this.Host;
        }
        public Socket ListenSocket;
        public Socket ConnectSocket;
        public int Connect(Node node)
        {
            Connection conn = new Connection();
            conn.Connector = this;
            IPAddress ip;
            IPEndPoint ep;
            conn.Node = node;
            IPAddress.TryParse(this.Host, out ip);
            ep = new IPEndPoint(ip, this.Port);
            this.ConnectSocket = this.createsock();
            conn.Socket = this.ConnectSocket;
            try
            {
                this.ConnectSocket.Connect(ep);
                conn.Auth();
            }
            catch
            {
                return 2;
            }
            return 1;
        }
        private Socket createsock()
        {
            AddressFamily af;
            Socket sock;
            switch (this.TransportProtocol)
            {
                case "IPV6":
                    af = AddressFamily.InterNetworkV6;
                    break;
                case "IPV4":
                    af = AddressFamily.InterNetwork;
                    break;
                default:
                    af = AddressFamily.InterNetwork;
                    break;
            }
            switch (this.Protocol)
            {
                case "udp":
                    sock = new Socket(af, SocketType.Dgram, ProtocolType.Udp);
                    break;
                case "tcp":
                    sock = new Socket(af, SocketType.Stream, ProtocolType.Tcp);
                    break;
                default:
                    sock = new Socket(af, SocketType.Stream, ProtocolType.Tcp);
                    break;
            }
            return sock;
        }
        public void Listen()
        {
            IPAddress ip;
            IPEndPoint ep;
            switch (this.Protocol) {
                case "udp":
                    this.ListenSocket = this.createsock();
                    IPAddress.TryParse(this.Host, out ip);
                    ep = new IPEndPoint(ip, this.Port);
                    Runner.DebugMessage("Debug.Info.Connector", this.Host + ":" + this.Port);
                    this.ListenSocket.Bind(ep);
                    break;
                case "tcp":
                    this.ListenSocket = this.createsock();
                    IPAddress.TryParse(this.Host, out ip);
                    ep = new IPEndPoint(ip, this.Port);
                    Runner.DebugMessage("Debug.Info.Connector", this.Host + ":" + this.Port);
                    this.ListenSocket.Bind(ep);
                    this.ListenSocket.Listen(20);
                    break;
            }
        }

    }

}
