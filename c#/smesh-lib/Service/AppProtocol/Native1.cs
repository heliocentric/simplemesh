
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
            this.Vampire = false;
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
           // authtypes.Add("NONE");
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
                Recieved = container.Receive(true);
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
            Node node;
            switch (authtotry)
            {
                case "RSA":
                    if (Runner.Network.NodeList.TryGetValue(Parameters["node.uuid"], out node) == true)
                    {
                        container.Node = node;
                        node.Version = Parameters["version"];

                        if (listen == true)
                        {
                            TextMessage bmsg = new TextMessage("Control.Auth.Challenge");
                            byte[] cookie = new byte[64];
                            Runner.Network.Random.NextBytes(cookie);
                            foreach (KeyValuePair<UUID, Auth> auth in node.AuthKeyList)
                            {
                                string ciphertext;
                                IMessage rmsg = auth.Value.Key.Encrypt(true, cookie, out ciphertext);
                                if (rmsg.Type == "Error.OK")
                                {
                                    string firstmessage = auth.Key + "!" + ciphertext;
                                    rmsg = Runner.Network.Node.Key.Encrypt(false, UTF8Encoding.UTF8.GetBytes(firstmessage), out ciphertext);
                                    bmsg.Data = Runner.Network.Node.Key.UUID.ToString() + "!" + ciphertext;
                                    this.Send(bmsg);
                                    Boolean ending = false;
                                    while (ending == false)
                                    {
                                        TextMessage tmsg;
                                        tmsg = new TextMessage(this.Receive(true));
                                        Boolean denied = false;
                                        switch (tmsg.Type)
                                        {
                                            case "Control.Auth.Response":
                                                string[] chunks = tmsg.Data.Split('!');
                                                ciphertext = chunks[1];
                                                byte[] bytes;
                                                Runner.Network.Node.Key.Decrypt(true, ciphertext, out bytes);
                                                chunks = UTF8Encoding.UTF8.GetString(bytes).Split('!');
                                                foreach (KeyValuePair<UUID, Auth> authtoken in node.AuthKeyList)
                                                {
                                                    if (authtoken.Key.ToString() == chunks[0])
                                                    {
                                                        authtoken.Value.Key.Decrypt(false, chunks[1], out bytes);
                                                        for (int i = 0; i < 64; i++)
                                                        {
                                                            if (cookie[i] != bytes[i]) {
                                                                denied = true;
                                                                break;
                                                            }
                                                        }
                                                        break;
                                                    }
                                                }
                                                if (denied == false)
                                                {
                                                    if (Runner.Network.NodeList.TryGetValue(Parameters["node.uuid"], out node) == true)
                                                    {
                                                        container.Node = node;
                                                        node.Version = Parameters["version"];
                                                        TextMessage omsg = new TextMessage("Control.Auth.OK");
                                                        omsg.Data = "test";
                                                        omsg.Sequence = tmsg.Sequence;
                                                        this.Send(omsg);
                                                        retval = 0;
                                                        ending = true;
                                                    }
                                                    else
                                                    {
                                                        retval = 1;
                                                    }
                                                }

                                                break;
                                        }
                                        }
                                }
                                else
                                {
                                    Runner.DebugMessage("Debug.Info.Auth", rmsg.Type);
                                }
                            }
                        }
                        else
                        {
                            bool ending = false;
                            while (ending == false)
                            {
                                IMessage rmsg;
                                rmsg = this.Receive(true);
                                switch (rmsg.Type)
                                {
                                    case "Control.Auth.Challenge":
                                        TextMessage Challenge = new TextMessage(rmsg);
                                        string[] chunks = Challenge.Data.Split('!');
                                        UUID uuidauth = new UUID(chunks[0]);
                                        IMessage response;
                                        foreach (KeyValuePair<UUID, Auth> auth in node.AuthKeyList)
                                        {
                                            if (auth.Key.ToString() == uuidauth.ToString())
                                            {
                                                byte[] firststage;
                                                response = auth.Value.Key.Decrypt(false, chunks[1], out firststage);
                                                if (response.Type == "Error.OK")
                                                {
                                                    byte[] output;
                                                    chunks = UTF8Encoding.UTF8.GetString(firststage).Split('!');
                                                    response = Runner.Network.Node.Key.Decrypt(true, chunks[1], out output);
                                                    byte[] plaintext = new byte[64];
                                                    for (int i = 0; i < 64; i++)
                                                    {
                                                        plaintext[i] = output[i];
                                                    }                                                    Challenge.Type = "Control.Auth.Response";
                                                    string ciphertext;
                                                    response = Runner.Network.Node.Key.Encrypt(false, plaintext, out ciphertext);
                                                    if (response.Type == "Error.OK")
                                                    {
                                                        ciphertext = Runner.Network.Node.Key.UUID + "!" + ciphertext;
                                                        response = auth.Value.Key.Encrypt(true, UTF8Encoding.UTF8.GetBytes(ciphertext), out ciphertext);
                                                        if (response.Type == "Error.OK")
                                                        {
                                                            ciphertext = auth.Key.ToString() + "!" + ciphertext;
                                                            Challenge.Data = ciphertext;
                                                            this.Send(Challenge);
                                                            ending = true;
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            retval = 1;
                                                            ending = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        retval = 1;
                                                        ending = true;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                }
                            }
                            ending = false;
                            while (ending == false)
                            {
                                IMessage rmsg = this.Receive(true);
                                switch (rmsg.Type)
                                {
                                    case "Control.Auth.OK":
                                        container.Node = node;
                                        node.Version = Parameters["version"];
                                        retval = 0;
                                        ending = true;
                                        break;
                                    default:
                                        retval = 1;
                                        ending = true;
                                        break;

                                }
                            }
                        }
                        Runner.DebugMessage("Debug.Info.Auth", "Remote Node is: " + node.ToString());
                        retval = 0;
                    }
                    else
                    {
                        retval = 1;
                    }
                    break;
                case "NONE":
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
            try
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
                if (this.Vampire == true)
                {
                    zombie = " - VAMPIRE";
                }
                return Connector.Protocol.ToString() + ": " + Socket.LocalEndPoint.ToString() + direction + Socket.RemoteEndPoint.ToString() + zombie;
            }
            catch
            {
                this.Vampire = true;
                return "VAMPIRE";
            }
        }

        public IMessage Send(IMessage msg)
        {
            IConnection args = this;
            IMessage retval = new Message();
            byte[] packed;
            packed = this.MessagePack(msg);
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
                args.Vampire = true;
            }
            return retval;
        }
        
        public IMessage Receive(Boolean ignore)
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
                    args.Vampire = true;

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
                        if (plength > 0)
                        {
                            byte[] payload = new byte[plength];
                            try
                            {
                                count = args.Socket.Receive(payload);
                            }
                            catch
                            {
                                retval.Type = "Error.Message.Corrupted";
                                args.Vampire = true;
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
                                args.Vampire = true;
                                return retval;
                            }
                        }
                        break;
                    case 0:
                        retval.Type = "Control.Empty";
                        break;
                    default:
                        retval.Type = "Error.Message.Corrupted";
                        args.Vampire = true;
                        break;

                }


            }
            MsgOut("R", retval);

            if (ignore == false)
            {
                switch (retval.Type)
                {
                    case "Control.GetTypeID":
                        if (retval.Conversation == 0)
                        {
                            MType type = new MType();
                            TextMessage msg = new TextMessage(retval);
                            type.Name = msg.Data;
                            type.TypeID = msg.Sequence;
                            msg.Type = "Control.TypeID";
                            if (this.TypeList.Contains(type) == false)
                            {
                                if (this.TypeList.ContainsID(type) == true)
                                {
                                    type.TypeID = 0;
                                    ushort value;
                                    this.TypeList.Add(type, out value);
                                    msg.Sequence = value;
                                    this.Send(msg);
                                }
                                else
                                {
                                    this.TypeList.Add(type);
                                    this.Send(msg);
                                }
                            }
                            
                        }
                        retval = new Message("Control.Empty");
                        break;
                    case "Control.Ping":
                        retval.Type = "Control.Pong";
                        this.Send(retval);
                        retval = new Message("Control.Empty");
                        break;
                    case "Control.Pong":
                        lock (this)
                        {
                            Time time;
                            if (this.OutstandingPings.TryGetValue(retval.Sequence, out time))
                            {
                                this.OutstandingPings.Remove(retval.Sequence);
                            }
                        }
                        retval = new Message("Control.Empty");
                        break;
                    default:
                        if ((retval.Type.Length == 6) && (retval.Type.Substring(0, 6) == "Error."))
                        {

                        }
                        else
                        {
                        }
                        break;

                }
            }

            return retval;

        }

        public IMessage Maintenence()
        {
            IMessage retval = new Message();
            TextMessage msg = new TextMessage("Control.Ping");
            if (this.OutstandingPings.Count < 10)
            {
                UInt16 newpingcount;
                if (this.PingCount == 65535)
                {
                    newpingcount = 0;
                }
                else
                {
                    newpingcount = this.PingCount++;
                }
                msg.Sequence = newpingcount;
                Time timestamp = new Time();
                msg.Data = timestamp.ToString();
                this.OutstandingPings.Add(msg.Sequence, timestamp);
                retval = this.Send(msg);
            }
            else
            {
                this.Vampire = true;
            }
            return retval;
        }

        public byte[] MessagePack(IMessage message)
        {
            IConnection args = this;
            byte[] rv = new byte[1];
            rv[0] = 0xFF;
            byte[] header = new byte[8];
            byte[] bytescratch;
            bytescratch = SimpleMesh.Utility.ToNetworkOrder(message.DataLength);
            header[0] = bytescratch[0];
            header[1] = bytescratch[1];
            bytescratch = SimpleMesh.Utility.ToNetworkOrder(message.Conversation);
            header[2] = bytescratch[0];
            header[3] = bytescratch[1];
            bytescratch = SimpleMesh.Utility.ToNetworkOrder(args.TypeList.ByName(message.Type).TypeID);
            header[4] = bytescratch[0];
            header[5] = bytescratch[1];
            bytescratch = SimpleMesh.Utility.ToNetworkOrder(message.Sequence);
            header[6] = bytescratch[0];
            header[7] = bytescratch[1];
            if (message.DataLength > 0)
            {
                rv = new byte[header.Length + message.DataLength];
                System.Buffer.BlockCopy(header, 0, rv, 0, header.Length);
                System.Buffer.BlockCopy(message.Payload, 0, rv, header.Length, message.DataLength);
            }
            else
            {
                rv = new byte[header.Length];
                System.Buffer.BlockCopy(header, 0, rv, 0, header.Length);
            }
            return rv;
        }

        public int Register(MType type)
        {
            int retval = 99;
            if (this.TypeList.Contains(type) == false)
            {
                TextMessage message = new TextMessage("Control.GetTypeID");
                message.Sequence = type.TypeID;
                message.Data = type.Name;
                this.Send(message);
                retval = 0;
            }

            return retval;
        }
        public int Deregister(MType type)
        {
            int retval = 99;
            return retval;
        }

        public static void MsgOut(string s, IMessage msg)
        {
            switch (msg.Type)
            {
                    /*
                case "Control.Ping":
                case "Control.Pong":
                     */
                case "Control.Auth.Challenge":
                case "Control.Auth.Response":
                case "Control.Empty":
                    break;
                default:
                    Runner.DebugMessage("Debug.Info.Message", s + ": " + msg.ToString());
                    break;

            }
        }
    }
}
