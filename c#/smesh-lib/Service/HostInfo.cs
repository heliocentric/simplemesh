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
                SimpleMesh.Service.Runner.DebugMessage("Debug.Listener", conn.Value.ToString());
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
