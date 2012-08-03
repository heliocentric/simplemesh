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
                Thread.Sleep(12000);
                foreach (KeyValuePair<string, Node> node in this.NodeList)
                {
                    foreach (Connection conn in node.Value.Connections)
                    {
                        Runner.DebugMessage("Debug.Info.Ping", "Attempting to connect to: " + node.Value.ToString());
                        lock (conn)
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
                                if (retval.Type.Substring(0, 6) == "Error.")
                                {
                                    node.Value.Cleanup(conn);
                                }
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
}
