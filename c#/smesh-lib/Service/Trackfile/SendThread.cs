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
        private Thread _SendThread;

        public Thread SendThread
        {
            get { return _SendThread; }
            set { _SendThread = value; }
        }

        public void Send()
        {
            SendThread = new Thread(new ThreadStart(this.SendWorker));
            SendThread.Start();
        }

        public void SendWorker()
        {
        }
    }
}
