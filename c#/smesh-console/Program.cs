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
using SimpleMesh;
using SimpleMesh.Service;
using SimpleMesh.Service.AppProtocol;

namespace MeshBroker
{
    class Program
    {
        static public void Version() {
            Console.WriteLine("Service Version: " + SimpleMesh.Service.Utility.Version);
            Console.WriteLine("Library Version: " + SimpleMesh.Utility.Version);
        }
        static void Main(string[] args)
        {
            Program.Version();
            SimpleMesh.Service.Runner.DebugMessageCallback = Program.ConsoleDebugMessage;
            SimpleMesh.Service.Runner.HostInfoCallback = Program.HostInfoQuery;
            SimpleMesh.Service.Runner.NetworkSpecCallback = Program.NetworkSpecification;

            bool end = false;
            bool started = false;


            Runner.DebugMode = "99";
            SimpleMesh.Service.Runner.Start();
            started = true;
            while (end == false)
            {
                Console.Write("ROOT:> ");
                string line = Console.ReadLine();
                string[] words = line.Split(' ');
                switch (words[0].ToLower())
                {
                    case "start":
                        if (started == false)
                        {
                            SimpleMesh.Service.Runner.Start();
                            started = true;
                        }
                        break;
                    case "stop":
                        if (started == true)
                        {
                            SimpleMesh.Service.Runner.Stop();
                            started = false;
                        }
                        break;
                    case "restart":
                        if (started == true)
                        {
                            SimpleMesh.Service.Runner.Stop();
                            started = false;
                        }
                        if (started == false)
                        {
                            SimpleMesh.Service.Runner.Start();
                            started = true;
                        }
                        break;
                    case "quit":
                        if (started == true)
                        {
                            SimpleMesh.Service.Runner.Stop();
                            started = false;
                        }
                        end = true;
                        break;
                    case "ver":
                    case "version":
                        Program.Version();
                        break;
                    case "debug":
                        if (words[1] != null)
                        {
                            Runner.DebugMode = words[1];
                        }
                        break;
                    case "set":
                        switch (words.Length)
                        {
                            case 2:
                                switch (words[1].ToLower())
                                {
                                    case "pingtime":
                                        Console.WriteLine("pingtime" + " = " + Runner.PingTime);
                                        break;
                                }
                                break;
                            case 3:
                                switch (words[1].ToLower())
                                {
                                    case "pingtime":
                                        Runner.PingTime = Convert.ToInt32(words[2]);
                                        Console.WriteLine("pingtime" + " = " + Runner.PingTime);
                                        break;
                                }
                                break;
                        }
                        break;
                    case "connect":
                        if (words.Length > 1)
                        {
                            string port = "17555";
                            string uuid = words[1];
                            if (words.Length > 2)
                            {
                                string host = words[2];
                                if (words.Length > 3)
                                {
                                    port = words[3];
                                }
                                else
                                {
                                }
                            }
                            else
                            {
                                if (uuid.Length == 36)
                                {
                                    Runner.Network.Connect(new UUID(uuid));
                                }
                                else
                                {
                                    Runner.Network.Connect(uuid);
                                }
                            }
                        }
                        break;
                    case "show":
                        if (words.Length > 1)
                        {
                            switch (words[1])
                            {
                                case "listen":
                                    lock (Runner.Network.Node) {
                                        foreach (KeyValuePair<string,Connector> conn in Runner.Network.Node.Connectors)
                                        {
                                            if (conn.Value.Zombie == false)
                                            {
                                                Console.WriteLine(conn.Value.ToString());
                                            }
                                        }
                                    }
                                    break;
                                case "node":
                                    if (words.Length > 2)
                                    {
                                        if (words[2] == Runner.Network.Node.UUID.ToString())
                                        {
                                            Program.DisplayNodeInfo(Runner.Network.Node);
                                        }
                                        else
                                        {
                                            Node scratch;
                                            if (Runner.Network.NodeList.TryGetValue(words[2], out scratch) == true)
                                            {
                                                Program.DisplayNodeInfo(Runner.Network.NodeList[words[2]]);
                                            }
                                        }
                                    }
                                    else
                                    {

                                        Console.WriteLine(Runner.Network.Node.UUID.ToString() + " = " + Runner.Network.Node.Description);
                                        foreach (KeyValuePair<string, Node> node in Runner.Network.NodeList)
                                        {
                                            Console.WriteLine(node.Value.UUID.ToString() + " = " + node.Value.Name);
                                        }
                                    }
                                    break;
                                case "conn":
                                    Runner.DebugMessage("Debug.Info.Show", "Listing connections");
                                    foreach (KeyValuePair<string, Node> node in Runner.Network.NodeList)
                                    {
                                        lock (node.Value.Connections)
                                        {
                                            Console.WriteLine(node.Value.ToString());
                                            foreach (IConnection conn in node.Value.Connections)
                                            {
                                                Console.WriteLine("\t" + conn.ToString());
                                            }
                                        }
                                    }
                                    break;
                                case "help":
                                case "?":
                                default:
                                    Console.WriteLine("conn\t\t\tShow Connection Information");
                                    Console.WriteLine("node\t\t\tShow Node Information");
                                    break;
                            }
                        }
                        else
                        {

                            Console.WriteLine("conn\t\t\tShow Connection Information");
                            Console.WriteLine("node\t\t\tShow Node Information");
                        }
                        break;
                    case "receive":
                        if (words.Length > 1)
                        {
                            
                        }
                        break;
                    case "register":
                        if (words.Length > 1)
                        {

                        }
                        break;
                    case "send":
                        break;
                    case "?":
                    case "help":
                        if (words.Length > 1)
                        {

                            Console.WriteLine("show\tnode\t\tShow node information");
                        }
                        else
                        {
                            Console.WriteLine("start\t\t\tStart SimpleMesh engine.");
                            Console.WriteLine("stop\t\t\tStop SimpleMesh engine.");
                            Console.WriteLine("restart\t\t\tRestart SimpleMesh engine.");
                            Console.WriteLine("show\t\t\tShow information.");
                            Console.WriteLine("debug\t\t\tChange the amount of debug info to show.");
                            Console.WriteLine("version\t\t\tSimpleMesh library and service version.");
                            Console.WriteLine("quit\t\t\tGracefully shut down SimpleMesh.");
                            Console.WriteLine("send\t\t\tSend a message.");
                            Console.WriteLine("receive\t\t\tRegister to receive messages.");
                        }
                        break;
                }
            }
        }
        
        public static void DisplayNodeInfo(HostInfo host)
        {
            lock (host)
            {
                lock (host.UUID)
                {
                    Console.WriteLine("UUID = " + host.UUID.ToString());
                }
                lock (host.Name)
                {
                    Console.WriteLine("Name = " + host.Name);
                }
                lock (host.Key)
                {
                    Console.WriteLine("Auth Key = " + host.Key.ToString());
                }
                Console.WriteLine("Connectors:");
                lock (host.Connectors)
                {
                    foreach (KeyValuePair<string, Connector> conn in host.Connectors)
                    {
                        lock (conn.Value)
                        {
                            Console.WriteLine("\t" + conn.Value.ToString());
                        }
                    }
                }
            }
        }
        public static void DisplayNodeInfo(Node node)
        {
            lock (node)
            {
                lock (node.UUID)
                {
                    Console.WriteLine("UUID = " + node.UUID.ToString());
                }
                lock (node.Name)
                {
                    Console.WriteLine("Name = " + node.Name);
                }

                Console.WriteLine("Keys:");
                lock (node.AuthKeyList)
                {
                    foreach (KeyValuePair<UUID, Auth> key in node.AuthKeyList)
                    {
                        Console.WriteLine("\t" + key.Value.ToString());
                    }
                }
                Console.WriteLine("Connectors:");
                lock (node.ConnectorList)
                {
                    foreach (KeyValuePair<string, Connector> conn in node.ConnectorList)
                    {
                        lock (conn.Value)
                        {
                            Console.WriteLine("\t" + conn.Value.ToString());
                        }
                    }
                }
            }
        }
        public static Dictionary<string, string> NetworkSpecification()
        {
            Dictionary<string, string> Hints;
            Hints = new Dictionary<string, string>();
            string answer;
            while (1 != 2)
            {
                Console.WriteLine("No Trackfile found!");
                Console.WriteLine("");
                Console.WriteLine("1) Create new network");
                Console.WriteLine("2) Enroll in existing network");
                Console.WriteLine("");
                Console.Write("> ");
                answer = Console.ReadLine();
                switch (answer)
                {
                    case "1":
                        Hints.Add("type", "create");
                        break;
                    case "2":
                        Hints.Add("type", "enroll");
                        break;
                }
                string throwaway;
                if (Hints.TryGetValue("type", out throwaway) == true)
                {
                    break;
                }
            }
            switch (Hints["type"])
            {
                case "create":
                    Console.WriteLine("");
                    Console.Write("Network Name or Description: ");
                    Hints.Add("name", Console.ReadLine());
                    Console.Write("Enrollment Key Length: ");
                    Hints.Add("enrollkeylength", Console.ReadLine());
                    Console.Write("Trackfile Filename: ");
                    Hints.Add("trackfilelocation", Console.ReadLine());
                    break;
                case "enroll":
                    Console.WriteLine("");
                    Console.Write("Trackfile Filename: ");
                    Hints.Add("trackfilelocation", Console.ReadLine());
                    break;
            }
            return Hints;
        }
        public static HostInfo HostInfoQuery()
        {
            HostInfo scratch = new HostInfo();
            scratch.Key = new Key();
            scratch.UUID = new UUID();

            
            Console.Write("Host Description: ");
            scratch.Description = Console.ReadLine();
            Console.Write("Key Length: ");
            string temp = Console.ReadLine();
            try
            {
                scratch.Key.Length = Convert.ToInt32(temp);
            }
            catch
            {
                scratch.Key.Length = 4096;
            }
            scratch.Ports.Add("17555");
            scratch.Protocols.Add("tcp");
            return scratch;
        }
        public static void ConsoleDebugMessage(string type, string message)
        {
            Console.WriteLine(message + "");
        }
    }
}
