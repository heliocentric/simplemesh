using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;

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
                lock (this.Node)
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
                    Connection acceptargs = new Connection();
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

    }
}
