﻿/*
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

using System.Net.NetworkInformation;
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
    public static partial class Runner
    {
        static string  _DatabasePath;
        static string _StorePath;
        private static Boolean _native;
        public static Boolean Native
        {
            get
            {
                    return _native;
               
            }
            set
            {
                _native = value;
            }
        }
        private static Trackfile _Network;
        public static Trackfile Network
        {
            get { return Runner._Network; }
            set { Runner._Network = value; }
        }
        private static string _DebugMode;

        public static string DebugMode
        {
            get
            {
                if (Runner._DebugMode == null)
                {
                    Runner._DebugMode = "1";
                }
                return Runner._DebugMode;
            }
            set { Runner._DebugMode = value; }
        }
        public delegate void DebugMessageType(string type, string value);
        public static DebugMessageType DebugMessageCallback;
        public static void DebugMessage(string type, string main) {
            switch (Runner.DebugMode)
            {
                case "0":
                case "1":
                    if (type.Length > 11)
                    {
                        if (type.Substring(0, 11) == "Debug.Info.")
                        {
                            return;
                        }
                    }
                    break;
                default:
                    DebugMessageCallback(type, main);
                    break;
            }
        }
        public static Int32 PingTime;
        public delegate HostInfo HostInfoType();
        public static HostInfoType HostInfoCallback;

        public delegate Dictionary<string, string> NetworkSpec();
        public static NetworkSpec NetworkSpecCallback;

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
        public static HostInfo Info {
            get
            {
                return Network.Node;
            }
            set
            {
                Network.Node = value;
            }
        }
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
                            if (chunks.Length > 2)
                            {
                                SimpleMesh.Service.Runner.Info.Description = chunks[2];
                            }
                            SimpleMesh.Service.Runner.DebugMessage("Debug.Info.ConfigFile", "\tUUID:\t\t" + SimpleMesh.Service.Runner.Info.UUID.ToString());
                            SimpleMesh.Service.Runner.DebugMessage("Debug.Info.ConfigFile", "\tDescription:\t\t" + SimpleMesh.Service.Runner.Info.Description);
                            break;
                        case "HOSTKEY":
                            SimpleMesh.Service.Runner.Info.Key = new Key();
                            SimpleMesh.Service.Runner.Info.Key.Decode(line.Replace("HOSTKEY!",""));
                            SimpleMesh.Service.Runner.DebugMessage("Debug.Info.ConfigFile", "\tHost Key:\t\t" + SimpleMesh.Service.Runner.Info.Key.ToString());
                            break;
                        case "PORT":
                            SimpleMesh.Service.Runner.Info.Ports.Add(chunks[1]);
                            break;
                        case "PROTOCOL":
                            SimpleMesh.Service.Runner.Info.Protocols.Add(chunks[1]);
                            break;
                    }
                }
            }
            else
            {
                // Get info from the actual client.
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
                file.Add("I!1.1!!!");
                file.Add("HOSTUUID!" + Info.UUID.ToString() + "!" + SimpleMesh.Service.Runner.Info.Description);
                file.Add("HOSTKEY!" + Info.Key.Encode(true));
                foreach(string port in Info.Ports) {
                    file.Add("PORT!" + port);
                }
                foreach (string protocol in Info.Protocols)
                {
                    file.Add("PROTOCOL!" + protocol);
                }

                SimpleMesh.Service.Runner.DebugMessage("Debug.Info.ConfigFile", SimpleMesh.Service.Runner.ConfigFile + " Contents:");
                foreach (string line in file)
                {
                    SimpleMesh.Service.Runner.DebugMessage("Debug.Info.ConfigFile", "\t" + line);
                }
                if (System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(SimpleMesh.Service.Runner.ConfigFile)) == false)
                {
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(SimpleMesh.Service.Runner.ConfigFile));
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
            string basepath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData) + System.IO.Path.DirectorySeparatorChar + @"NiftyEngineering";
            string configfile = basepath + System.IO.Path.DirectorySeparatorChar + @"config.tkf";
            SimpleMesh.Service.Runner.Start(basepath, configfile);
        }
        public static bool Mono;
        public static void Start(string path, string configfile)
        {
            Runner.PingTime = 4373;
            Type t = Type.GetType("Mono.Runtime");
            if (t != null)
            {
                Runner.Mono = true;
            }
            else
            {
                Runner.Mono = false;
            }
            SimpleMesh.Service.Runner.StorePath = path;
            SimpleMesh.Service.Runner.ConfigFile = configfile;

            SimpleMesh.Service.Runner.Network = new Trackfile();
            SimpleMesh.Service.Runner.Read();
            try
            {
                NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface adapter in adapters)
                {
                    IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                    foreach (IPAddressInformation unicast in adapterProperties.UnicastAddresses)
                    {
                        if (unicast.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            string[] chunks = unicast.Address.ToString().Split('.');
                            if (chunks[0] != "169")
                            {
                                Info.Addresses.Add(unicast.Address);
                            }
                        }
                        else
                        {
                            Info.Addresses.Add(unicast.Address);
                        }
                    }

                }
            }
            catch
            {
            }
            if (Info.Addresses.Count == 0)
            {
                try
                {

                    System.Net.IPHostEntry myiphost = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                    foreach (System.Net.IPAddress myipadd in myiphost.AddressList)
                    {
                        Info.Addresses.Add(myipadd);
                    }

                }
                catch
                {
                    if (Runner.Mono == true)
                    {
                        Info.Addresses.Add(IPAddress.Any);
                        Info.Addresses.Add(IPAddress.IPv6Any);
                    }

                }
            }

            Info.Compile();
            SimpleMesh.Service.Runner.Write();
            SimpleMesh.Service.Runner.DebugMessage("Debug.Info.ConfigFile", SimpleMesh.Service.Runner.StorePath);
            SimpleMesh.Service.Runner.DebugMessage("Debug.Info.ConfigFile", SimpleMesh.Service.Runner.ConfigFile);
            string TrackfilePath = SimpleMesh.Service.Runner.StorePath + System.IO.Path.DirectorySeparatorChar + @"default.tkf";
            Runner.DebugMessage("Debug.Info.Trackfile", "Trackfile Path: " + TrackfilePath);
            if (System.IO.File.Exists(TrackfilePath) == true)
            {
                SimpleMesh.Service.Runner.Network.Read(TrackfilePath);
            }
            else
            {
                Dictionary<string, string> Hints = SimpleMesh.Service.Runner.NetworkSpecCallback();
                string type;
                string trackfilelocation;
                if (Hints.TryGetValue("trackfilelocation", out trackfilelocation) == false)
                {
                    return;
                }
                if (Hints.TryGetValue("type", out type) == true)
                {
                    switch (type)
                    {
                        case "create":
                            string name;
                            if (Hints.TryGetValue("name", out name) == true)
                            {
                                SimpleMesh.Service.Runner.Network.Name = name;
                            }
                            else
                            {
                                SimpleMesh.Service.Runner.Network.Name = "Undefined";
                            }
                            string keylength;
                            Key scratch = new Key();
                            if (Hints.TryGetValue("enrollkeylength", out keylength) == true)
                            {
                                scratch.Generate("RSA", keylength);
                            }
                            else
                            {
                                scratch.Generate("RSA");
                            }
                            Auth temp = new Auth();
                            temp.UUID = new UUID();
                            temp.Key = scratch;
                            SimpleMesh.Service.Runner.Network.Enrollment.Add(temp.UUID.ToString(), temp);
                            break;

                        case "enroll":
                            Network.ReadOnce(trackfilelocation);
                            break;
                    }
                    try
                    {
                        Network.WriteOnce(trackfilelocation);
                    }
                    catch
                    {
                    }
                }

            }
            SimpleMesh.Service.Runner.Network.Write(TrackfilePath);
            /*
            string inputstring = "Test";
            byte[] ciphertext;
            byte[] plaintext;
            Network.Node.Key.Encrypt(true, UTF8Encoding.UTF8.GetBytes(inputstring), out ciphertext);
            Network.Node.Key.Decrypt(true, ciphertext, out plaintext);
            Console.WriteLine("output string = " + UTF8Encoding.UTF8.GetString(plaintext));
            Console.ReadLine();
            */
            Runner.Native = true;
            Network.Start();

        }
        public static void Stop()
        {
            Network.Stop();
        }
    }
}
