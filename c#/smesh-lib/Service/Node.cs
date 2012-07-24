using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleMesh.Service
{
    public class Node
    {
        public Dictionary<string, Connector> ConnectionList;
        public string UUID;
        public string Name;
        public string Description;
        public Dictionary<string, Auth> AuthKeyList;
        public List<MessageBrokerArgs> LiveConnectionList;
        public Node()
        {
            this.UUID = "";
            this.Name = "";
            this.Description = "";
            this.ConnectionList = new Dictionary<string, Connector>();
            this.AuthKeyList = new Dictionary<string, Auth>();
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
        public void Connect()
        {

        }
        public void Connect(Connector conn)
        {
        }
    }
}
