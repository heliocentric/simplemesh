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
        private Thread _ReceiveThread;

        public Thread ReceiveThread
        {
            get { return _ReceiveThread; }
            set { _ReceiveThread = value; }
        }
        public void Receive()
        {
            this.ReceiveThread = new Thread(new ThreadStart(this.ReceiveWorker));
            this.ReceiveThread.Start();
        }
        public void ReceiveWorker()
        {
            List<Socket> ReadList;
            bool end = false;
            while (end == false)
            {
                ReadList = new List<Socket>();
                foreach (KeyValuePair<string, Node> node in this.NodeList)
                {
                    lock (node.Value)
                    {
                        foreach (IConnection conn in node.Value.Connections)
                        {
                            lock (conn)
                            {
                                if (conn.Zombie == false)
                                {
                                    ReadList.Add(conn.Socket);
                                }
                            }
                        }
                    }
                }
                if (ReadList.Count == 0)
                {
                    Thread.Sleep(500);
                    continue;
                }
                Socket.Select(ReadList, null, null, 1000);
                foreach (Socket sock in ReadList)
                {
                    foreach (KeyValuePair<string, Node> node in this.NodeList)
                    {
                        IConnection realconn = null;
                        lock (node.Value)
                        {
                            foreach (IConnection conn in node.Value.Connections)
                            {
                                if (conn.Socket == sock)
                                {
                                    realconn = conn;
                                }
                            }
                        }
                        if (realconn != null)
                        {
                            try
                            {
                                Receiver((object) realconn);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
        }
        public void Receiver(object obj)
        {
            IConnection conn = (IConnection)obj;
            IMessage message;
             message = conn.Receive();
            
            switch (message.Type)
            {
                case "Control.Ping":
                    message.Type = "Control.Pong";
                    conn.Send(message) ;
                    break;
                case "Control.Pong":
                    lock (conn)
                    {
                        Time time;
                        if (conn.OutstandingPings.TryGetValue(message.Sequence, out time))
                        {
                            conn.OutstandingPings.Remove(message.Sequence);
                        }
                    }
                    break;
                default:
                    if ((message.Type.Length == 6) && (message.Type.Substring(0, 6) == "Error."))
                    {

                    }
                    else
                    {
                    }
                    break;

            }

        }
    }
}
