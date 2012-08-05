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
            List<Connection> staleconns;
            while (end == false)
            {

                Thread.Sleep(19531);
                lock (Runner.Network.NodeList)
                {
                    foreach (KeyValuePair<string, Node> node in Runner.Network.NodeList)
                    {
                        staleconns = new List<Connection>();
                        lock (node.Value.Connections)
                        {
                            foreach (Connection conn in node.Value.Connections)
                            {
                                if (conn.Zombie == true)
                                {
                                    staleconns.Add(conn);
                                }
                            }

                            foreach (Connection conn in staleconns)
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
