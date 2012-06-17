using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SimpleMesh.Service;

namespace SimpleMesh.Service
{
    class Trackfile
    {
        Dictionary<string, Node> NodeList;
        Dictionary<string, Auth> Enrollment;
        private string filename;
        private string _versiontype;
        private string _createddate;
        private string _lastmodifieddate;
        public Trackfile()
        {
            this.NodeList = new Dictionary<string, Node>();
            this.Enrollment = new Dictionary<string, Auth>();
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
                    }
                }
                else
                {
                    switch (this._versiontype)
                    {
                        case "1.0":
                            string[] chunks = line.Split('!');
                            string UUID;
                            Node scratch;
                            switch (chunks[0])
                            {
                                case "#":
                                    break;
                                case "EMPTY":
                                    break;
                                case "N":
                                    UUID = chunks[1];
                                    scratch = this.NodeInit(UUID);
                                    scratch.Name = chunks[2];
                                    break;
                                case "C":
                                    UUID = chunks[1];
                                    scratch = this.NodeInit(UUID);
                                    Connector scratchconnector = new Connector();
                                    scratchconnector.AppProtocol = chunks[2];
                                    scratchconnector.Protocol = chunks[3];
                                    scratchconnector.Port = chunks[4];
                                    scratchconnector.Host = chunks[5];
                                    scratch.ConnectionList.Add(scratchconnector.Key(), scratchconnector);
                                    break;
                                case "A":
                                    UUID = chunks[1];
                                    scratch = this.NodeInit(UUID);
                                    scratch.AuthKeyList.Add(chunks[2], new Auth(chunks[2], chunks[3], BoolUnpack(chunks[4]), BoolUnpack(chunks[5])));
                                    break;
                                case "E":
                                    UUID = chunks[1];
                                    this.Enrollment.Add(UUID, new Auth(chunks[2], chunks[3], BoolUnpack(chunks[4]), BoolUnpack(chunks[5])));
                                    break;
                            }
                            break;
                    }
                }
            }
            string message = "Enrollment Keys";
            foreach(KeyValuePair<string, Auth> enrolled in this.Enrollment)
            {
                message += "\n\t" + enrolled.Value.Type;
                message += "\n\t\t" + enrolled.Value.Token;
            }
            MessageBox.Show(message);
            foreach(KeyValuePair<string, Node> line in this.NodeList) {
                Node scratch = line.Value;
                message = "Name=\t" + scratch.Name;
                message += "\nUUID=\t" + scratch.UUID;
                message += "\nAuthentication:";
                foreach (KeyValuePair<string, Auth> auth in scratch.AuthKeyList)
                {
                    message += "\n\t" + auth.Value.Type.ToString() + "=";
                    message += "\n\t\t" + auth.Value.Token.ToString();
                }
                message += "\nConnectors:";
                foreach (KeyValuePair<string, Connector> connector in scratch.ConnectionList)
                {
                    message += "\n\t" + connector.Value.ToString();
                }
                MessageBox.Show(message);
            }
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
            return this.WriteFile(filename);
        }
        private int WriteFile(string filename)
        {
            List<String> FileContents;
            FileContents = new List<string>();
            FileContents.Add("I!1.0!" + this._createddate + "!" + Utility.ToUnixTimestamp(System.DateTime.Now));
            FileContents.Add("# SimpleMesh Trackfile version 1.0");
            FileContents.Add("");
            FileContents.Add("");
            FileContents.Add("# E!UUID!AuthType!AuthToken!Active (1/0)!Primary (1/0)!");
            FileContents.Add("");
            foreach (KeyValuePair<string, Auth> item in this.Enrollment)
            {
                FileContents.Add("E!" + item.Key + "!" + item.Value.Type + "!" + item.Value.Token + "!" + this.BoolPack(item.Value.Active) + "!" + this.BoolPack(item.Value.Primary));
            }
            FileContents.Add("");
            FileContents.Add("");
            FileContents.Add("# N!UUID!Name");
            FileContents.Add("");
            foreach (KeyValuePair<string, Node> item in this.NodeList)
            {
                FileContents.Add("N!" + item.Key + "!" + item.Value.Name);
            }

            FileContents.Add("");
            FileContents.Add("");
            FileContents.Add("# A!UUID!AuthType!AuthToken!Active (1/0)!Primary (1/0)!");
            FileContents.Add("");
            foreach (KeyValuePair<string, Node> item in this.NodeList)
            {
                foreach (KeyValuePair<string, Auth> auth in item.Value.AuthKeyList)
                {
                    FileContents.Add("A!" + item.Key + "!" + auth.Key + "!" + auth.Value.Token + "!" + this.BoolPack(auth.Value.Active) + "!" + this.BoolPack(auth.Value.Primary));
                }
                FileContents.Add("");
            }
            FileContents.Add("");
            FileContents.Add("");
            FileContents.Add("# C!UUID!AppProtocol!Protocol!Port!Host");
            FileContents.Add("");
            foreach (KeyValuePair<string, Node> item in this.NodeList)
            {
                foreach (KeyValuePair<string, Connector> connector in item.Value.ConnectionList)
                {
                    FileContents.Add("C!" + item.Key + "!" + connector.Value.ToString());
                }
                FileContents.Add("");
            }
            System.IO.File.WriteAllLines(filename, FileContents.ToArray());
            return 0;
        }
    }
    class Node
    {
        public Dictionary<string, Connector> ConnectionList;
        public string UUID;
        public string Name;
        public string Description;
        public Dictionary<string, Auth> AuthKeyList;
        public Node()
        {
            this.UUID = "";
            this.Name = "";
            this.Description = "";
            this.ConnectionList = new Dictionary<string,Connector>();
            this.AuthKeyList = new Dictionary<string, Auth>();
        }
    }
    class Auth
    {
        public string Type;
        public string Token;
        public Boolean Active;
        public Boolean Primary;
        public Auth()
        {
        }
        public Auth(string type, string token)
        {
            this.Type = type;
            this.Token = token;
            this.Active = true;
            this.Primary = false;
        }
        public Auth(string type, string token, Boolean active, Boolean primary)
        {
            this.Type = type;
            this.Token = token;
            this.Active = active;
            this.Primary = primary;
        }
        public override string ToString()
        {
            return this.Type + "=" + this.Token;
        }
    }
    class Connector
    {
        public string Protocol;
        public string AppProtocol;
        public string Host;
        public string Port;
        public Connector()
        {
            this.Protocol = "";
            this.AppProtocol = "";
            this.Host = "";
            this.Port = "";
        }
        public Connector(string connspec)
        {
            this.Protocol = "";
            this.AppProtocol = "";
            this.Host = "";
            this.Port = "";
        }
        public override string ToString()
        {
            return this.AppProtocol + "!" + this.Key();
        }
        public string Key()
        {
            return this.Protocol + "!" + this.Port + "!" + this.Host;
        }

    }
}
