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
        }
    }
}
