using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleMesh.Service.AppProtocol
{
    public class Native1 : StubConnection, IConnection
    {
        public Native1()
        {
            this.TypeList = new TypeList();
            this.Connect = false;
            this.OutstandingPings = new Dictionary<ushort, Time>();
            this.PingCount = (ushort)Runner.Network.Random.Next(0, 65000);
            this.Zombie = false;
        }


        private Boolean _connect;

        public Boolean Connect
        {
            get { return _connect; }
            set { _connect = value; }
        }
        public int Auth()
        {
            return this.Auth(false);
        }
        public int Auth(bool listen)
        {
            return this.Auth(listen, false);
        }
        public int Auth(bool listen, bool enrollment)
        {
            IConnection container = this;
            int retval = 99;
            KeyValueMessage keyval;
            List<string> authtypes = new List<string>();
            authtypes.Add("NONE");
            authtypes.Add("RSA");

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

            keyval = new KeyValueMessage("Control.PreAuth");
            keyval.Add("node.uuid", Runner.Network.Node.UUID.ToString());
            keyval.Add("auth.types", authstring);
            keyval.Add("version", SimpleMesh.Service.Utility.Version);
            if (listen == false)
            {
                Utility.SendMessage(container, keyval);
            }
            bool end;
            end = false;
            bool error = false;
            IMessage Recieved;
            bool uuid = false;
            bool authreceived = false;
            bool enrolling = false;
            KeyValueMessage messages;
            List<string> typelist;
            typelist = new List<string>();
            string conntype;
            Dictionary<string, string> Parameters = new Dictionary<string, string>();
            while (end == false)
            {
                Recieved = Utility.ReceiveMessage(container);
                if (Recieved.Type.Substring(0, 6) != "Error.")
                {
                    switch (Recieved.Type)
                    {
                        case "Control.PreAuth":
                            messages = new KeyValueMessage(Recieved);
                            foreach (KeyValuePair<string, string> keyn in messages.Data)
                            {
                                switch (keyn.Key)
                                {
                                    case "node.uuid":
                                        uuid = true;
                                        break;
                                    case "auth.types":
                                        authreceived = true;
                                        string[] types = keyn.Value.Split(' ');
                                        foreach (string type in types)
                                        {
                                            if (authtypes.Contains(type))
                                            {
                                                typelist.Add(type);
                                            }
                                        }
                                        break;
                                }
                                Parameters.Add(keyn.Key, keyn.Value);
                            }
                            if (listen == true)
                            {
                                Utility.SendMessage(container, keyval);
                            }
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
                    if (Runner.Network.NodeList.TryGetValue(Parameters["node.uuid"], out node) == true)
                    {
                        container.Node = node;
                        node.Version = Parameters["version"];
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
        public override string ToString()
        {
            string direction = "";
            if (Connect == true)
            {
                direction = " -> ";
            }
            else
            {
                direction = " <- ";
            }
            string zombie = "";
            if (this.Zombie == true)
            {
                zombie = " - ZOMBIE";
            }
            return Connector.Protocol.ToString() + ": " + Socket.LocalEndPoint.ToString() + direction + Socket.RemoteEndPoint.ToString() + zombie;
        }
    }
}
