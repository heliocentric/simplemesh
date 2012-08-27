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

        IMessage Send(IMessage Message);
    }
}
