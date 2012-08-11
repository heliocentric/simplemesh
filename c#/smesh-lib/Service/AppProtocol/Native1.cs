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
                container.Send(keyval);
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
                Recieved = container.Receive();
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
                                container.Send(keyval);
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

        public IMessage Send(IMessage msg)
        {
            IConnection args = this;
            IMessage retval = new Message();
            byte[] packed;
            packed = Utility.MessagePack(msg, args);
            try
            {
                switch (args.Connector.Protocol)
                {
                    case "tcp":

                        MsgOut("T", msg);
                        args.Socket.Send(packed);
                        break;
                }
                retval.Type = "Control.OK";
            }
            catch
            {
                retval.Type = "Error.Socket.Send";
                args.Zombie = true;
            }
            return retval;
        }
        
        public IMessage Receive()
        {
            IConnection args = this;
            IMessage retval = new Message();
            byte[] header = new byte[8];
            int count;
            lock (args)
            {
                count = 0;
                try
                {
                    count = args.Socket.Receive(header);
                }
                catch
                {
                    retval.Type = "Error.Message.Corrupted";
                    args.Zombie = true;

                }
                switch (count)
                {
                    case 8:
                        byte[] _length = new byte[2];
                        byte[] _conversation = new byte[2];
                        byte[] _typeid = new byte[2];
                        byte[] _sequence = new byte[2];

                        _length[0] = header[0];
                        _length[1] = header[1];
                        _conversation[0] = header[2];
                        _conversation[1] = header[3];
                        _typeid[0] = header[4];
                        _typeid[1] = header[5];
                        _sequence[0] = header[6];
                        _sequence[1] = header[7];


                        UInt16 plength;
                        UInt16 pconversation;
                        UInt16 ptypeid;
                        UInt16 psequence;

                        plength = SimpleMesh.Utility.ToHostOrder(_length);
                        pconversation = SimpleMesh.Utility.ToHostOrder(_conversation);
                        ptypeid = SimpleMesh.Utility.ToHostOrder(_typeid);
                        psequence = SimpleMesh.Utility.ToHostOrder(_sequence);

                        byte[] payload = new byte[plength];
                        try
                        {
                            count = args.Socket.Receive(payload);
                        }
                        catch
                        {
                            retval.Type = "Error.Message.Corrupted";
                            args.Zombie = true;
                            return retval;
                        }
                        if (count == plength)
                        {
                            retval.Sequence = psequence;
                            retval.Conversation = pconversation;
                            retval.Type = args.TypeList.ByID(ptypeid).Name;
                            retval.Payload = payload;
                        }
                        else
                        {
                            retval.Type = "Error.Message.Corrupted";
                            args.Zombie = true;
                            return retval;
                        }
                        break;
                    case 0:
                        retval.Type = "Control.Empty";
                        break;
                    default:
                        retval.Type = "Error.Message.Corrupted";
                        args.Zombie = true;
                        break;

                }


            }
            MsgOut("R", retval);
            return retval;

        }
        
        public static void MsgOut(string s, IMessage msg)
        {
            switch (msg.Type)
            {
                case "Control.Ping":
                case "Control.Pong":
                case "Control.Empty":
                    break;
                default:
                    Runner.DebugMessage("Debug.Info.Message", s + ": " + msg.ToString());
                    break;

            }
        }
    }
}
