using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleMesh.Service
{
    public interface IConnector
    {
        Message Send(Message Name);
        Message Recieve();
        TypeList Types
        {
            get;
            set;
        }
    }
}
