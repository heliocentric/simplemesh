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
                        foreach (Connection conn in node.Value.Connections)
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
                        Connection realconn = null;
                        lock (node.Value)
                        {
                            foreach (Connection conn in node.Value.Connections)
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
            Connection conn = (Connection)obj;
            IMessage message;
             message = Utility.ReceiveMessage(conn);
            
            switch (message.Type)
            {
                case "Control.Ping":
                    message.Type = "Control.Pong";
                    Utility.SendMessage(conn, message);
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

            }

        }
    }
}
