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
        static void Main(string[] args)
        {
            Console.WriteLine("Service Version: " + SimpleMesh.Service.Utility.Version);
            Console.WriteLine("Library Version: " + SimpleMesh.Utility.Version);
            SimpleMesh.Service.Runner.DebugMessageCallback = Program.ConsoleDebugMessage;
            SimpleMesh.Service.Runner.HostInfoCallback = Program.HostInfoQuery;
            SimpleMesh.Service.Runner.Start();
            Console.ReadLine();
        }
        public static HostInfo HostInfoQuery()
        {
            HostInfo scratch = new HostInfo();
            scratch.Key = new Key();
            scratch.UUID = new UUID();

            
            Console.Write("\n\rHost Description: ");
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
            Console.Write(message + "\n\r");
        }
    }
}
