/*
# Copyright 2011-2012 Dylan Cochran
# All rights reserved
#
# Redistribution and use in source and binary forms, with or without
# modification, are permitted providing that the following conditions
# are met:
# 1. Redistributions of source code must retain the above copyright
#    notice, SimpleMesh.Service.Runner list of conditions and the following disclaimer.
# 2. Redistributions in binary form must reproduce the above copyright
#    notice, SimpleMesh.Service.Runner list of conditions and the following disclaimer in the
#    documentation and/or other materials provided with the distribution.
#
# SimpleMesh.Service.Runner SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
# IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
# WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
# ARE DISCLAIMED.  IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY
# DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
# DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS
# OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
# HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
# STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING
# IN ANY WAY OUT OF THE USE OF SimpleMesh.Service.Runner SOFTWARE, EVEN IF ADVISED OF THE
# POSSIBILITY OF SUCH DAMAGE.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections.ObjectModel;

namespace SimpleMesh.Service
{
    public class TriggerList<T>
    {
    }
    public class HostInfo
    {
        public string Description;
        public Key Key;
        public UUID UUID;
        public List<string> Ports;
        public List<string> Protocols;
        public Dictionary<string, Connector>  Connectors;
        public List<IPAddress> Addresses;
        public HostInfo() {
            this.Ports = new List<string>();
            this.Protocols = new List<string>();
            this.Addresses = new List<IPAddress>();
            this.Connectors = new Dictionary<string, Connector>();
        }
        public void Compile() {
            foreach (IPAddress ip in this.Addresses) {
                foreach(string protocol in this.Protocols) {
                    foreach(string port in this.Ports) {
                        Connector scratch = new Connector();
                        scratch.AppProtocol = "native1";
                        scratch.Port = Convert.ToInt32(port);
                        scratch.Protocol = protocol;
                        scratch.Host = ip.ToString();
                        scratch.HostType = "ADDRESS";
                        switch (ip.AddressFamily) {
                            case System.Net.Sockets.AddressFamily.InterNetwork:
                                scratch.TransportProtocol = "IPV4";
                                break;
                            case System.Net.Sockets.AddressFamily.InterNetworkV6:
                                scratch.TransportProtocol = "IPV6";
                                break;
                        }
                        this.Connectors.Add(scratch.Key(), scratch);
                    }
                }
            }
            foreach (KeyValuePair<string, Connector> conn in this.Connectors)
            {
                SimpleMesh.Service.Runner.DebugMessage("Debug.Info.Listener", conn.Value.ToString());
            }
        }
        public void Listen()
        {
            foreach (KeyValuePair<string, Connector> conn in this.Connectors)
            {
                conn.Value.Listen();
            }
        }
    }
}
