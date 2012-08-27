using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleMesh
{
    public class MaxList
    {
        private Dictionary<String, UInt64> _reallist;
        public MaxList()
        {
            this._reallist = new Dictionary<string, ulong>();
            /* 
             * native1 is our minimum
             */
            this._reallist.Add("Conversation", 65536);
            this._reallist.Add("Payload", 65527);
            this._reallist.Add("Sequence", 65536);
            this._reallist.Add("Types", 65536);
        }
        public UInt64 Get(string key)
        {
            return this._reallist[key];
        }
    }
}
