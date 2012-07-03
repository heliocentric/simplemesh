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
using System.IO;
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
    public class Runner
    {
        string _DatabasePath;
        string _StorePath;
        Trackfile Network;
        public string DatabasePath
        {
            get { return _DatabasePath; }
            set { _DatabasePath = value; }
        }
        public string StorePath
        {
            get { return _StorePath; }
            set { _StorePath = value; }
        }
        public Key HostKey;
        public UUID Node;
        public int Read()
        {
            if (System.IO.File.Exists(this.ConfigFile) == true)
            {
                string[] lines = System.IO.File.ReadAllLines(this.ConfigFile);
                Utility.DebugMessage("Debug.Info.ConfigFile", this.ConfigFile + " Contents:");
                foreach (string line in lines)
                {
                    string[] chunks = line.Split('!');
                    switch (chunks[0])
                    {
                        case "HOSTUUID":
                            this.Node = new UUID(chunks[1]);
                            Utility.DebugMessage("Debug.Info.ConfigFile", "\tUUID:\t\t" + this.Node.ToString());
                            break;
                        case "HOSTKEY":
                            this.HostKey = new Key();
                            this.HostKey.Decode(line.Replace("HOSTKEY!",""));
                            Utility.DebugMessage("Debug.Info.ConfigFile", "\tHost Key:\t\t" + this.HostKey.ToString());
                            break;
                    }
                }
            }
            else
            {
                // Generate Template configuration file.
                this.Node = new UUID();
                DateTime Start = DateTime.Now;
                this.HostKey = new Key();
                this.HostKey.Type = "RSA";
                this.HostKey.Length = 1024;
                Utility.DebugMessage("Debug.Info.ConfigFile", "Generating " + this.HostKey.Length.ToString() + " bit RSA key pair starting on: " + DateTime.Now.ToString());
                RsaKeyPairGenerator r = new RsaKeyPairGenerator();
                r.Init(new KeyGenerationParameters(new SecureRandom(), this.HostKey.Length));
                this.HostKey.BouncyPair = r.GenerateKeyPair();

                List<string> file = new List<string>();
                file.Add("I!1.0!!!");
                file.Add("HOSTUUID!" + this.Node.ToString());
                file.Add("HOSTKEY!" + this.HostKey.Encode());

                Utility.DebugMessage("Debug.Info.ConfigFile", this.ConfigFile + " Contents:");
                foreach (string line in file)
                {
                    Utility.DebugMessage("Debug.Info.ConfigFile", "\t" + line);
                }
                System.IO.File.WriteAllLines(this.ConfigFile, file.ToArray());
            }
            return 0;
        }
        public int Write()
        {
            return 0;
        }
        public string ConfigFile;
        public void Start()
        {
            string basepath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData) + @"\NiftyEngineering";
            string configfile = basepath + @"\config.tkf";
            this.Start(basepath, configfile);
        }
        public void Start(string path, string configfile)
        {
            this.StorePath = path;
            this.ConfigFile = configfile;
            this.Read();
            /*
            Utility.DebugMessage("Debug.Info.ConfigFile", this.StorePath);
            Utility.DebugMessage("Debug.Info.ConfigFile", this.ConfigFile);
            Utility.DebugMessage("Debug.Info.ConfigFile", this.Node.ToString());
            this.Network = new Trackfile();
            string TrackfilePath = this.StorePath + @"\default.tkf";
            if (System.IO.File.Exists(TrackfilePath) == true)
            {
                this.Network.Read(TrackfilePath);
            }
            else
            {

            }
           this.Network.Write(TrackfilePath);

            
            SimpleMesh.Service.Net.TCP scratch = new SimpleMesh.Service.Net.TCP();
            scratch.Listen();
             */
        }
    }
}
