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
                    case "?":
                    case "help":
                        Console.WriteLine("start\t\t\tStart SimpleMesh engine.");
                        Console.WriteLine("stop\t\t\tStop SimpleMesh engine.");
                        Console.WriteLine("restart\t\t\tRestart SimpleMesh engine.");
                        Console.WriteLine("version\t\t\tSimpleMesh library and service version.");
                        Console.WriteLine("quit\t\t\tGracefully shut down SimpleMesh.");
                        break;
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
            scratch.Ports.Add("17556");
            scratch.Ports.Add("17557");
            scratch.Protocols.Add("udp");
            scratch.Protocols.Add("tcp");
            return scratch;
        }
        public static void ConsoleDebugMessage(string type, string message)
        {
            Console.WriteLine(message + "");
        }
    }
}
