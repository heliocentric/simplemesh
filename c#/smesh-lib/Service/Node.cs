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
using SimpleMesh.Service.AppProtocol;

namespace SimpleMesh.Service
{
    public class Node
    {
        public Dictionary<string, Connector> ConnectorList;
        public string UUID;
        public string Name;
        public string Description;
        public string Version;
        public Dictionary<string, Auth> AuthKeyList;
        public List<IConnection> Connections;
        public Node()
        {
            this.UUID = "";
            this.Name = "";
            this.Description = "";
            this.ConnectorList = new Dictionary<string, Connector>();
            this.AuthKeyList = new Dictionary<string, Auth>();
            this.Connections = new List<IConnection>();
        }
        public override string ToString()
        {
            return this.UUID.ToString() + "!" + this.Name.ToString() + "!" + this.Description.ToString();
        }
        public string Encode()
        {
            return this.ToString();
        }
        public void Decode(string nodeline)
        {

        }
        public int Connect()
        {
            int retval;
            retval = 1;
            foreach (KeyValuePair<string, Connector> conn in this.ConnectorList) {
                Runner.DebugMessage("Debug.Info.Connect", "Attempting to connect to " + conn.Value.ToString());
                retval = this.Connect(conn.Value);
                if (retval == 0)
                {
                    return retval;
                }
                else
                {
                    continue;
                }
            }
            return retval;
        }
        public void Cleanup(IConnection conn)
        {
        }
        public int Connect(Connector conn)
        {
            int retval;
            retval = conn.Connect(this);
            return retval;
        }
    }
}
