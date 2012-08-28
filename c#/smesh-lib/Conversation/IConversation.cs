using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleMesh;

namespace SimpleMesh
{
    public interface IConversation
    {
        MaxList MaxList
        {
            get;
            set;
        }
        string Tag
        {
            get;
            set;
        }
        IMessage Send(IMessage Message);
        IConversation NewConversation(string Tag);
        IConversation NewConversation(string Tag, UUID Node);
    }
}
