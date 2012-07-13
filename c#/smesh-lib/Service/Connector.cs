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
    public interface IConnector
    {
        Socket ListenSocket {
            get;
            set;
        }
    }
    public class Connector
    {
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
       
        public void Listen()
        {
            AddressFamily af;
            IPAddress ip;
            IPEndPoint ep;
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
            switch (this.Protocol) {
                case "udp":
                    this.ListenSocket = new Socket(af, SocketType.Dgram, ProtocolType.Udp);
                    IPAddress.TryParse(this.Host, out ip);
                    ep = new IPEndPoint(ip, this.Port);
                    this.ListenSocket.Bind(ep);
                    break;
                case "tcp":
                    this.ListenSocket = new Socket(af, SocketType.Stream, ProtocolType.Tcp);
                    IPAddress.TryParse(this.Host, out ip);
                    ep = new IPEndPoint(ip, this.Port);
                    this.ListenSocket.Bind(ep);
                    this.ListenSocket.Listen(20);
                    break;
            }
        }

    }

}
