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
    }
}
