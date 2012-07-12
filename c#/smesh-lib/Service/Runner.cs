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
using System.Net;

namespace SimpleMesh.Service
{
    public static class Runner
    {

        static string  _DatabasePath;
        static string _StorePath;
        static Trackfile Network;


        public delegate void DebugMessageType(string type, string value);
        public static DebugMessageType DebugMessageCallback;
        public static void DebugMessage(string type, string main) {
            DebugMessageCallback(type, main);
        }

        public delegate HostInfo HostInfoType();
        public static HostInfoType HostInfoCallback;

        public static string DatabasePath
        {
            get { return _DatabasePath; }
            set { _DatabasePath = value; }
        }
        public static string StorePath
        {
            get { return _StorePath; }
            set { _StorePath = value; }
        }
        public static HostInfo Info;
        public static int Read()
        {
            Info = new HostInfo();

            if (System.IO.File.Exists(SimpleMesh.Service.Runner.ConfigFile) == true)
            {
                string[] lines = System.IO.File.ReadAllLines(SimpleMesh.Service.Runner.ConfigFile);
                SimpleMesh.Service.Runner.DebugMessage("Debug.Info.ConfigFile", SimpleMesh.Service.Runner.ConfigFile + " Contents:");
                foreach (string line in lines)
                {
                    string[] chunks = line.Split('!');
                    switch (chunks[0])
                    {
                        case "HOSTUUID":
                            SimpleMesh.Service.Runner.Info.UUID = new UUID(chunks[1]);
                            SimpleMesh.Service.Runner.DebugMessage("Debug.Info.ConfigFile", "\tUUID:\t\t" + SimpleMesh.Service.Runner.Info.UUID.ToString());
                            break;
                        case "HOSTKEY":
                            SimpleMesh.Service.Runner.Info.Key = new Key();
                            SimpleMesh.Service.Runner.Info.Key.Decode(line.Replace("HOSTKEY!",""));
                            SimpleMesh.Service.Runner.DebugMessage("Debug.Info.ConfigFile", "\tHost Key:\t\t" + SimpleMesh.Service.Runner.Info.Key.ToString());
                            break;
                    }
                }
            }
            else
            {
                // Generate Template configuration file.
                HostInfo scratch = HostInfoCallback();
                DateTime Start = DateTime.Now;
                SimpleMesh.Service.Runner.Info = scratch;
                SimpleMesh.Service.Runner.Info.Key.Type = "RSA";
                SimpleMesh.Service.Runner.DebugMessage("Debug.Info.ConfigFile", "Generating " + SimpleMesh.Service.Runner.Info.Key.Length.ToString() + " bit RSA key pair starting on: " + DateTime.Now.ToString());
                RsaKeyPairGenerator r = new RsaKeyPairGenerator();
                r.Init(new KeyGenerationParameters(new SecureRandom(), SimpleMesh.Service.Runner.Info.Key.Length));
                AsymmetricCipherKeyPair test = r.GenerateKeyPair();
                SimpleMesh.Service.Runner.Info.Key.PrivateKey = test.Private;
                SimpleMesh.Service.Runner.Info.Key.PublicKey = test.Public;

                List<string> file = new List<string>();
                file.Add("I!1.0!!!");
                file.Add("HOSTUUID!" + SimpleMesh.Service.Runner.Info.UUID.ToString());
                file.Add("HOSTKEY!" + SimpleMesh.Service.Runner.Info.Key.Encode());

                SimpleMesh.Service.Runner.DebugMessage("Debug.Info.ConfigFile", SimpleMesh.Service.Runner.ConfigFile + " Contents:");
                foreach (string line in file)
                {
                    SimpleMesh.Service.Runner.DebugMessage("Debug.Info.ConfigFile", "\t" + line);
                }
                System.IO.File.WriteAllLines(SimpleMesh.Service.Runner.ConfigFile, file.ToArray());
            }
            return 0;
        }
        public static int Write()
        {
            return 0;
        }
        public static string ConfigFile;
        public static void Start()
        {
            string basepath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData) + @"\NiftyEngineering";
            string configfile = basepath + @"\config.tkf";
            SimpleMesh.Service.Runner.Start(basepath, configfile);
        }
        public static void Start(string path, string configfile)
        {
            SimpleMesh.Service.Runner.StorePath = path;
            SimpleMesh.Service.Runner.ConfigFile = configfile;
            SimpleMesh.Service.Runner.Read();

            
            SimpleMesh.Service.Runner.DebugMessage("Debug.Info.ConfigFile", SimpleMesh.Service.Runner.StorePath);
            SimpleMesh.Service.Runner.DebugMessage("Debug.Info.ConfigFile", SimpleMesh.Service.Runner.ConfigFile);
            SimpleMesh.Service.Runner.DebugMessage("Debug.Info.ConfigFile", SimpleMesh.Service.Runner.Info.UUID.ToString());
            SimpleMesh.Service.Runner.Network = new Trackfile();
            string TrackfilePath = SimpleMesh.Service.Runner.StorePath + @"\default.tkf";
            if (System.IO.File.Exists(TrackfilePath) == true)
            {
                SimpleMesh.Service.Runner.Network.Read(TrackfilePath);
            }
            else
            {
            }
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork || ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    SimpleMesh.Service.Runner.DebugMessage("Debug.Info.ConfigFile", ip.ToString());
                }
            }
           SimpleMesh.Service.Runner.Network.Write(TrackfilePath);

            /*
            SimpleMesh.Service.Net.TCP scratch = new SimpleMesh.Service.Net.TCP();
            scratch.Listen();
             */
           
        }
    }
}
