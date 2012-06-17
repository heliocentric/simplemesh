using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleMesh.Service
{
    public class Type
    {
        public UInt16 TypeID;
        public string Name;
    }
    public class TypeList
    {
        public TypeList()
        {
            this._ByID = new Dictionary<ushort, Type>();
            this._ByName = new Dictionary<string, Type>();
            Type scratch;
            scratch = new Type();
            scratch.Name = "Control.GetTypeID";
            scratch.TypeID = 57005;
            this.Add(scratch);
            scratch = new Type();
            scratch.Name = "Control.TypeID";
            scratch.TypeID = 48879;
            this.Add(scratch);
            scratch = new Type();
            scratch.Name = "Control.Auth.UUID";
            scratch.TypeID = 6500;
            this.Add(scratch);
            scratch = new Type();
            scratch.Name = "Control.Auth.Challenge";
            scratch.TypeID = 6501;
            this.Add(scratch);
            scratch = new Type();
            scratch.Name = "Control.Auth.Response";
            scratch.TypeID = 6502;
            this.Add(scratch);
            scratch = new Type();
            scratch.Name = "Control.Auth.OK";
            scratch.TypeID = 6503;
            this.Add(scratch);
            scratch = new Type();
            scratch.Name = "Control.Auth.Failed";
            scratch.TypeID = 6504;
            this.Add(scratch);
        }
        private Dictionary<UInt16, Type> _ByID;
        private Dictionary<String, Type> _ByName;
        public Message Add(Type name) {
            _ByID.Add(name.TypeID, name);
            _ByName.Add(name.Name, name);
            return new Message();
        }
        public Message Remove(Type name)
        {
            return new Message();
        }
        public Type ByName(string name)
        {
            return _ByName[name];
        }
        public Type ByID(UInt16 id)
        {
            return _ByID[id];
        }
    }
}
