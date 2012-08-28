using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleMesh
{
    public class ControlConversation : IConversation
    {
        public ControlConversation(string ApplicationSignature)
        {
            this.MaxList = new MaxList();
        }
        private MaxList _maxlist;
        public MaxList MaxList
        {
            get
            {
                return this._maxlist;
            }
            set
            {
                this._maxlist = value;
            }
        }
        private string _tag;
        public string Tag
        {
            get
            {
                return _tag;
            }
            set
            {
                _tag = value;
            }
        }
        public IMessage Send(IMessage Message)
        {
            var retval = new TextMessage("Error.OK");
            if (SimpleMesh.Service.Runner.Native == true)
            {
                
            }
            else
            {

            }
            return retval;
        }
        public IConversation NewConversation(string Tag, UUID Node)
        {
            var retval = new Conversation();
            return retval;
        }
        public IConversation NewConversation(string Tag)
        {
            var retval = new Conversation();
            return retval;
        }
    }
}
