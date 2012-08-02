using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleMesh.Service
{
    public class Node
    {
        public Dictionary<string, Connector> ConnectorList;
        public string UUID;
        public string Name;
        public string Description;
        public Dictionary<string, Auth> AuthKeyList;
        public List<Connection> Connections;
        public Node()
        {
            this.UUID = "";
            this.Name = "";
            this.Description = "";
            this.ConnectorList = new Dictionary<string, Connector>();
            this.AuthKeyList = new Dictionary<string, Auth>();
            this.Connections = new List<Connection>();
        }
        public override string ToString()
        {
            return this.UUID.ToString() + "!" + this.Name.ToString() + "!" + this.Description.ToString();
        }
        public string Encode()
        {
            return this.ToString();
        }
        public void Decode(string nodeline)
        {

        }
        public int Connect()
        {
            int retval;
            retval = 1;
            foreach (KeyValuePair<string, Connector> conn in this.ConnectorList) {
                Runner.DebugMessage("Debug.Info.Connect", "Attempting to connect to " + conn.Value.ToString());
                retval = this.Connect(conn.Value);
                if (retval == 0)
                {
                    return retval;
                }
                else
                {
                    continue;
                }
            }
            return retval;
        }
        public int Connect(Connector conn)
        {
            int retval;
            retval = conn.Connect(this);
            return retval;
        }
    }
}
