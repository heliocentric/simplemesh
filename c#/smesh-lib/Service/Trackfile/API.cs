using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleMesh.Service
{
    public partial class Trackfile
    {
        public IMessage Send()
        {
            var retval = new TextMessage("Error.OK");

            return retval;
        }
        public void Register(string ApplicationSignature, string Type)
        {

        }
        public void DeRegister(string ApplicationSignature, string Type)
        {
        }
    }
}
