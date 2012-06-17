using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleMesh
{
    public class Network
    {
        public Network(string Name, string Trackfile)
        {
        }
        public Conversation NewConversation(string Token)
        {
            Conversation retval = new Conversation(Token);
            return retval;
        }
        public Conversation NewConversation(string Token, UUID Remote)
        {
            Conversation retval = new Conversation(Token, Remote);
            return retval;
        }
        public Conversation ControlConversation()
        {
            return new Conversation();
        }
    }
}
