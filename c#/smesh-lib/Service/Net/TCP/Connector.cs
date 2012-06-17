using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace SimpleMesh.Service.Net
{
    class TCP : IConnector
    {
        int Port;
        List<TcpListener> Listeners;
        List<Socket> Connections;
        Thread ListenThread;
        Thread RecieveThread;
        TypeList _Types;
        public TCP()
        {
            this.Listeners = new List<TcpListener>();
            this.Connections = new List<Socket>();
            this.Port = 17555;
            this.ListenThread = new Thread(new ThreadStart(this.ListenWorker));
            this.ListenThread.Start();
            this.RecieveThread = new Thread(new ThreadStart(this.RecieveWorker));
            this._Types = new TypeList();
        }
        public TypeList Types
        {
            get
            {
                return _Types;
            }
            set
            {
                _Types = value;
            }
        }
        public Message Recieve()
        {
            return new Message();
        }
        public Message Send(Message test)
        {
            return new Message();
        }
        public void Listen() {
            TcpListener main = new TcpListener(System.Net.IPAddress.Any, this.Port);
            main.Start();
            this.Listeners.Add(main);
        }
        private void ListenWorker()
        {
            Socket scratch;
            while (1 != 2)
            {
                foreach (TcpListener listener in this.Listeners)
                {
                    if (listener.Pending() == true)
                    {
                        scratch = listener.AcceptSocket();
                        this.Connections.Add(scratch);
                        System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(this.AcceptWorker), scratch);
                    }
                }
                Thread.Sleep(1000);
            }
        }
        private void AcceptWorker(Object socket)
        {
            Socket inputsocket = (Socket)socket;
            TextMessage scratch =new TextMessage("Control.Auth.UUID");
            scratch.Data = "abc";
            this.SendMessage(inputsocket, scratch);
            int breaker = 0;
            Message Received;
            string RemoteUUID;
            bool PassedAuth = false;
            while (breaker == 0) {
                Received = this.ReceiveMessage(inputsocket);
                switch (Received.Type)
                {
                    case "Control.Auth.UUID":
                        RemoteUUID = Encoding.UTF8.GetString(Received.Payload);
                        scratch.Type = "Control.Auth.Challenge";
                        scratch.Data = "test";
                        break;
                    case "Control.Auth.Response":
                        break;
                }
            }

            inputsocket.Close();
            this.Connections.Remove(inputsocket);
        }
        private void SendMessage(Socket socket, Message message)
        {
            byte[] packed = Utility.MessagePack(message, this.Types);
            Utility.DebugMessage(10, "T:" + " Length=" + packed.Length + " Type=" + message.Type + " Payload=" + Encoding.UTF8.GetString(message.Payload));
            socket.Send(packed);
        }
        private Message ReceiveMessage(Socket socket)
        {
            Message scratch = new Message();

            return scratch;
        }
        private void RecieveWorker()
        {

        }
    }
}
