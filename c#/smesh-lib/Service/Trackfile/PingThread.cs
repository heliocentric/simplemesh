using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;

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
                            foreach (Connection conn in node.Value.Connections)
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
                                            IMessage retval = Utility.SendMessage(conn, msg);
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
