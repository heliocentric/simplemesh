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
using System.Net.Sockets;
using System.Threading;
using SimpleMesh.Service.AppProtocol;

namespace SimpleMesh.Service
{
    public partial class Trackfile
    {
        private Thread _PingThread;

        public Thread PingThread
        {
            get { return _PingThread; }
            set { _PingThread = value; }
        }

        public void Ping()
        {
            this.PingThread = new Thread(new ThreadStart(this.PingWorker));
            this.PingThread.Start();
        }

        public void PingWorker()
        {
            bool end = false;
            while (end == false)
            {
                if (Runner.PingTime > 0)
                {
                    Thread.Sleep(Runner.PingTime);
                    foreach (KeyValuePair<string, Node> node in this.NodeList)
                    {
                        lock (node.Value.Connections)
                        {
                            foreach (IConnection conn in node.Value.Connections)
                            {
                                lock (conn)
                                {
                                    if (conn.Zombie == false)
                                    {
                                        TextMessage msg = new TextMessage("Control.Ping");
                                        if (conn.OutstandingPings.Count < 10)
                                        {
                                            UInt16 newpingcount;
                                            if (conn.PingCount == 65535)
                                            {
                                                newpingcount = 0;
                                            }
                                            else
                                            {
                                                newpingcount = conn.PingCount++;
                                            }
                                            msg.Sequence = newpingcount;
                                            Time timestamp = new Time();
                                            msg.Data = timestamp.ToString();
                                            conn.OutstandingPings.Add(msg.Sequence, timestamp);
                                            IMessage retval = conn.Send(msg);
                                        }
                                        else
                                        {
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
                else
                {
                    Thread.Sleep(20000);
                }
            }
        }
    }
}
