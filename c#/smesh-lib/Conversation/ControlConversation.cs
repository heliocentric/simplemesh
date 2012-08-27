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
        public MaxList _maxlist;
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
        public IMessage Send(IMessage Message)
        {
            retval = new TextMessage("Error.OK");
            return retval;
        }
    }
}
