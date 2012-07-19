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
using System.Windows.Forms;
using System.Net.Sockets;
using SimpleMesh.Service;
using System.Threading;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO.Pem;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;

namespace SimpleMesh.Service
{
    public class Trackfile
    {
        private HostInfo _Node;

        public HostInfo Node
        {
            get { return _Node; }
            set { _Node = value; }
        }
        private Dictionary<string, Node> _NodeList;

        public Dictionary<string, Node> NodeList
        {
            get { return _NodeList; }
            set { _NodeList = value; }
        }
        private Dictionary<string, Auth> _Enrollment;

        public Dictionary<string, Auth> Enrollment
        {
            get { return _Enrollment; }
            set { _Enrollment = value; }
        }
        private string filename;
        private string _versiontype;
        private string _createddate;
        private string _lastmodifieddate;
        private string _name;
        private string _nethash;
        private bool _new;
        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                this._name = value;
                if (this._new == true)
                {
                    SecureRandom test = new SecureRandom();
                    test.GenerateSeed(29);
                    int maxrounds = test.Next(2, 8);
                    string seed = "";
                    for (int rounds = 1; rounds <= maxrounds; rounds++)
                    {
                        seed += test.Next().ToString();
                    }
                    this._nethash = Utility.SHA256(seed + value);
                }
            }
        }
        public string Hash
        {
            get
            {
                return this._nethash;
            }
        }
        public string Filename
        {
            get
            {
                return this.filename;
            }
            set
            {
                this.filename = value;
            }
        }
        public Trackfile()
        {
            this.NodeList = new Dictionary<string, Node>();
            this.Enrollment = new Dictionary<string, Auth>();
            this._createddate = Utility.ToUnixTimestamp(System.DateTime.Now).ToString();
            this._lastmodifieddate = Utility.ToUnixTimestamp(System.DateTime.Now).ToString();
            this._versiontype = "1.0";
            this._new = true;
        }
        private Node NodeInit(string UUID)
        {
            Node scratch;
            if (this.NodeList.ContainsKey(UUID) == true)
            {
                scratch = this.NodeList[UUID];
            }
            else
            {
                scratch = new Node();
                scratch.UUID = UUID;
                this.NodeList.Add(UUID, scratch);
            }
            return scratch;
        }
        private Boolean BoolUnpack(string value)
        {
            if (value.Length == 0)
            {
                return false;
            }
            else
            {
                if (value == "1")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        private string BoolPack(Boolean value)
        {
            if (value == true)
            {
                return "1";
            }
            else
            {
                return "0";
            }
        }
        public int Read(string filename)
        {
            this.filename = filename;
            return this.ReadFile(filename);
        }
        public int Read()
        {
            return this.ReadFile(this.filename);
        }
        public int ReadOnce(string filename)
        {
            return this.ReadFile(filename);
        }
        private int ReadFile(string filename)
        {
            string[] configfile = System.IO.File.ReadAllLines(filename);
            Boolean firstline = true;
            foreach (string line in configfile)
            {
                string LineType;
                if (line.Length != 0)
                {
                    LineType = line.Substring(0, 1);
                }
                else
                {
                    LineType = "EMPTY";
                }
                if (firstline == true)
                {
                    firstline = false;
                    string[] chunks = line.Split('!');
                    if (LineType == "I")
                    {
                        this._versiontype = chunks[1];
                        this._createddate = chunks[2];
                        this._lastmodifieddate = chunks[3];
                        this._name = chunks[4];
                        this._nethash = chunks[5];
                    }
                }
                else
                {
                    switch (this._versiontype)
                    {
                        case "1.0":
                            string[] chunks = line.Split('!');
                            string UUID;
                            string keystring;
                            Node scratch;
                            switch (chunks[0])
                            {
                                case "#":
                                    break;
                                case "EMPTY":
                                    break;
                                case "N":
                                    UUID = chunks[1];
                                    if (UUID != Node.UUID.ToString())
                                    {
                                        scratch = this.NodeInit(UUID);
                                        scratch.Name = chunks[2];
                                    }
                                    break;
                                case "C":
                                    UUID = chunks[1];
                                    if (UUID != Node.UUID.ToString())
                                    {
                                        scratch = this.NodeInit(UUID);
                                        string connectorstring = line.Replace("C!" + chunks[1] + "!", "");
                                        Connector scratchconnector = new Connector(connectorstring);
                                        scratch.ConnectionList.Add(scratchconnector.Key(), scratchconnector);
                                    }
                                    break;
                                case "A":
                                    UUID = chunks[1];
                                    if (UUID != Node.UUID.ToString())
                                    {
                                        scratch = this.NodeInit(UUID);
                                        keystring = line.Replace("A!" + chunks[1] + "!" + chunks[2] + "!" + chunks[3] + "!", "");
                                        scratch.AuthKeyList.Add(keystring, new Auth(keystring, BoolUnpack(chunks[3]), BoolUnpack(chunks[4])));
                                    }
                                    break;
                                case "E":
                                    UUID = chunks[1];
                                    keystring = line.Replace("E!" + chunks[1] + "!" + chunks[2] + "!" + chunks[3] + "!", "");
                                    this.Enrollment.Add(UUID, new Auth(keystring, BoolUnpack(chunks[2]), BoolUnpack(chunks[3])));
                                    break;
                            }
                            break;
                    }
                }
            }
            string message = "Enrollment Keys";
            foreach(KeyValuePair<string, Auth> enrolled in this.Enrollment)
            {
                message += "\n\t" + enrolled.Value.Key.ToString();
            }
            SimpleMesh.Service.Runner.DebugMessage("Debug.Info.Trackfile", message);
            foreach(KeyValuePair<string, Node> line in this.NodeList) {
                Node scratch = line.Value;
                message = "Name=\t" + scratch.Name;
                message += "\nUUID=\t" + scratch.UUID;
                message += "\nAuthentication:";
                foreach (KeyValuePair<string, Auth> auth in scratch.AuthKeyList)
                {
                    message += "\n\t" + auth.Value.Key.ToString();
                }
                message += "\nConnectors:";
                foreach (KeyValuePair<string, Connector> connector in scratch.ConnectionList)
                {
                    message += "\n\t" + connector.Value.ToString();
                }
                SimpleMesh.Service.Runner.DebugMessage("Debug.Info.Trackfile", message);
            }
            this._new = false;
            return 0;
        }
        public int Write()
        {
            return this.WriteFile(this.filename);
        }
        public int Write(string filename)
        {
            this.filename = filename;
            return this.WriteFile(filename);
        }
        public int WriteOnce(string filename)
        {
            return this.WriteFile(filename, true);
        }
        private int WriteFile(string filename)
        {
            return this.WriteFile(filename, false);
        }
        private int WriteFile(string filename, bool EnrollmentKey)
        {
            switch (this._versiontype)
            {
                case "1.0":
                    List<String> FileContents;
                    FileContents = new List<string>();
                    FileContents.Add("I!1.0!" + this._createddate + "!" + Utility.ToUnixTimestamp(System.DateTime.Now) + "!" + this.Name + "!" + this.Hash);
                    FileContents.Add("# SimpleMesh Trackfile version 1.0");
                    foreach (KeyValuePair<string, Auth> item in this.Enrollment)
                    {
                        FileContents.Add("E!" + item.Key + "!" + this.BoolPack(item.Value.Active) + "!" + this.BoolPack(item.Value.Primary) + "!" + item.Value.Key.Encode(EnrollmentKey));
                    }
                    foreach (KeyValuePair<string, Node> item in this.NodeList)
                    {
                        FileContents.Add("N!" + item.Key + "!" + item.Value.Name);
                    }
                    HostInfo test = this.Node;
                    string value = "N!" + test.UUID.ToString() + "!" + test.Description;
                    FileContents.Add(value);
                    foreach (KeyValuePair<string, Node> item in this.NodeList)
                    {
                        foreach (KeyValuePair<string, Auth> auth in item.Value.AuthKeyList)
                        {
                            FileContents.Add("A!" + item.Key + "!" + this.BoolPack(auth.Value.Active) + "!" + this.BoolPack(auth.Value.Primary) + "!" + auth.Value.Key.Encode());
                        }
                    }
                    FileContents.Add("A!" + Node.UUID.ToString() + "!" + "1" + "!" + "1" + "!" + Node.Key.Encode(false));
                    foreach (KeyValuePair<string, Node> item in this.NodeList)
                    {
                        foreach (KeyValuePair<string, Connector> connector in item.Value.ConnectionList)
                        {

                                FileContents.Add("C!" + item.Key + "!" + connector.Value.ToString(this._versiontype));
                        }
                    }
                    foreach (KeyValuePair<string, Connector> connector in Node.Connectors)
                    {

                        switch (connector.Value.Host) {
                            case "127.0.0.1":
                                break;
                            case "::1":
                                break;
                            default:
                                FileContents.Add("C!" + this.Node.UUID.ToString() + "!" + connector.Value.ToString(this._versiontype));
                            break;
                        }
                    }
                    FileContents.Add("SIG!None!");
                    System.IO.File.WriteAllLines(filename, FileContents.ToArray());
                    break;
            }
            return 0;
        }
        private Thread _ListenThread;
        public Thread ListenThread
        {
            get
            {
                return this._ListenThread;
            }
            set
            {
                this._ListenThread = value;
            }
        }
        public void Listen()
        {
            this.ListenThread = new Thread(new ThreadStart(this.Listener));
            this.ListenThread.Start();
        }
        private void Listener()
        {
            this.Node.Listen();
            List<Socket> listenlist;
            bool end = false;
            while (end != true)
            {
                listenlist = new List<Socket>();
                lock (this.Node.Connectors)
                {
                    foreach (KeyValuePair<string, Connector> keypair in this.Node.Connectors)
                    {
                        listenlist.Add(keypair.Value.ListenSocket);
                    }
                }

                Socket.Select(listenlist, null, null, 1000);
                foreach (Socket sock in listenlist)
                {
                    MessageBrokerArgs acceptargs = new MessageBrokerArgs();
                    lock (this.Node.Connectors)
                    {
                        foreach (KeyValuePair<string, Connector> keypair in this.Node.Connectors)
                        {
                            if (sock == keypair.Value.ListenSocket)
                            {
                                acceptargs.Connector = keypair.Value;
                            }
                        }
                    }
                    
                    acceptargs.Socket = sock.Accept();
                    ThreadPool.QueueUserWorkItem(this.AcceptSocket, acceptargs);
                }
            }
        }
        public void Connect(string uuid, string host, string port)
        {
        }
        private void AcceptSocket(Object acceptargs)
        {
            MessageBrokerArgs container = (MessageBrokerArgs) acceptargs;
            string host = container.Socket.RemoteEndPoint.ToString();
            Runner.DebugMessage("Debug.Net.Listener", "Connection recieved from " + host);
            TextMessage scratch;
            scratch = new TextMessage("Control.Auth.UUID");
            scratch.Data = this.Node.UUID.ToString();
            Utility.SendMessage(container, scratch);

            List<string> authtypes = new List<string>();
            authtypes.Add("RSA");
            authtypes.Add("HASH-SHA256");
            authtypes.Add("HASH-MD5");

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
            Message Recieved;
            bool uuid = false;
            bool authreceived = false;
            UUID remoteuuid;
            TextMessage text;
            List<string> typelist;
            typelist = new List<string>();
            while (end == false)
            {
                Recieved = Utility.ReceiveMessage(container);
                switch (Recieved.Type)
                {
                    case "Control.Auth.UUID":
                        text = (TextMessage)Recieved;
                        remoteuuid = new UUID(text.Data);
                        uuid = true;
                        break;
                    case "Control.Auth.Types":
                        text = (TextMessage)Recieved;
                        
                        string [] types = text.Data.Split(' ');
                        foreach (string type in types) {
                            if (authtypes.Contains(type))
                            {
                                typelist.Add(type);
                            }
                        }
                        authreceived = true;
                        break;
                }
                if (authreceived == true && uuid == true)
                {
                    end = true;
                }
            }

        }
        public void Start()
        {
            this.Listen();
        }
        public void Stop()
        {
            if (this.ListenThread != null)
        {
                this.ListenThread.Abort();
            }
        }
    }
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
            this.ConnectionList = new Dictionary<string,Connector>();
            this.AuthKeyList = new Dictionary<string, Auth>();
        }
    }
    public class Auth
    {
        public Key Key;
        public UUID UUID;
        public Boolean Active;
        public Boolean Primary;
        public Auth()
        {
        }
        public Auth(string keystring)
        {

            this.Key = new Key(keystring);
            this.Active = true;
            this.Primary = false;
        }
        public Auth(string keystring, Boolean active, Boolean primary)
        {
            this.Key = new Key(keystring);
            this.Active = active;
            this.Primary = primary;
        }
        public override string ToString()
        {
            return this.Key.ToString();
        }
    }
    
}
