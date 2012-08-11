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
        private Thread _ListenThread;
        public Thread ListenThread
        {
            get
            {
                return this._ListenThread;
            }
            set
            {
                this._ListenThread = value;
            }
        }
        public void Listen()
        {
            this.ListenThread = new Thread(new ThreadStart(this.Listener));
            this.ListenThread.Start();
        }
        private void Listener()
        {
            this.Node.Listen();
            List<Socket> listenlist;
            bool end = false;
            while (end != true)
            {
                listenlist = new List<Socket>();
                lock (this.Node.Connectors)
                {
                    foreach (KeyValuePair<string, Connector> keypair in this.Node.Connectors)
                    {
                        listenlist.Add(keypair.Value.ListenSocket);
                    }
                }
                if (listenlist.Count == 0)
                {
                    end = true;
                    continue;
                }
                Socket.Select(listenlist, null, null, 1000);
                foreach (Socket sock in listenlist)
                {
                    IConnection acceptargs = new Native1();
                    lock (this.Node.Connectors)
                    {
                        foreach (KeyValuePair<string, Connector> keypair in this.Node.Connectors)
                        {
                            if (sock == keypair.Value.ListenSocket)
                            {
                                acceptargs.Connector = keypair.Value;
                            }
                        }
                    }
                    acceptargs.Socket = sock.Accept();
                    ThreadPool.QueueUserWorkItem(this.AcceptSocket, acceptargs);
                }
            }
        }

        private void AcceptSocket(Object acceptargs)
        {
            IConnection container = (IConnection)acceptargs;
            string host = container.Socket.RemoteEndPoint.ToString();
            Runner.DebugMessage("Debug.Net.Listener", "Connection recieved from " + host);
            int retval = container.Auth(true);
            if (retval == 0)
            {
                lock (container.Node.Connections)
                {
                    container.Node.Connections.Add(container);
                }
            }

        }

    }
}
