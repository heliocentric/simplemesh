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
        }
    }
}
