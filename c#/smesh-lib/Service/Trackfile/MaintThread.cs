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
using System.Threading;
using System.Net.Sockets;
using SimpleMesh.Service.AppProtocol;

namespace SimpleMesh.Service
{
    public partial class Trackfile
    {
        private Thread _MaintThread;

        public Thread MaintThread
        {
            get { return _MaintThread; }
            set { _MaintThread = value; }
        }

        public void Maint()
        {
            MaintThread = new Thread(new ThreadStart(this.MaintWorker));
            MaintThread.Start();
        }

        public void MaintWorker()
        {
            bool end;
            end = false;
            List<IConnection> staleconns;
            while (end == false)
            {

                Thread.Sleep(5000);
                lock (Runner.Network.NodeList)
                {
                    foreach (KeyValuePair<string, Node> node in Runner.Network.NodeList)
                    {
                        staleconns = new List<IConnection>();
                        lock (node.Value.Connections)
                        {
                            foreach (IConnection conn in node.Value.Connections)
                            {
                                if (conn.Zombie == true)
                                {
                                    staleconns.Add(conn);
                                }
                                else
                                {
                                    conn.Maintenence();
                                }
                            }

                            foreach (IConnection conn in staleconns)
                            {
                                node.Value.Connections.Remove(conn);
                            }
                        }
                        if (node.Value.Connections.Count < 2)
                        {
                            node.Value.Connect();
                        }
                    }
                }

            }
        }
    }
}
