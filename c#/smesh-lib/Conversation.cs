using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleMesh
{
    public class Conversation
    {
        public Conversation()
        {
        }
        public Conversation(string Token)
        {
            this.Token = Token;
        }
        public Conversation(string Token, UUID Remote)
        {
            this.Token = Token;
            this.Remote = Remote;
        }
        private UUID _remote;

        public UUID Remote
        {
            get { return _remote; }
            set { _remote = value; }
        }

        private string _token;

        public string Token
        {
            get { return _token; }
            set { _token = value; }
        }
        public void Send(Message scratch) {
        }
        public Message Recieve()
        {
            System.Threading.Thread.Sleep(10 * 100);
            return new Message();
        }
    }
}
