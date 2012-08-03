﻿/*
# Copyright 2011-2012 Dylan Cochran
# All rights reserved
#
# Redistribution and use in source and binary forms, with or without
# modification, are permitted providing that the following conditions
# are met:
# 1. Redistributions of source code must retain the above copyright
#    notice, this list of conditions and the following disclaimer.
# 2. Redistributions in binary form must reproduce the above copyright
#    notice, this list of conditions and the following disclaimer in the
#    documentation and/or other materials provided with the distribution.
#
# THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
# IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
# WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
# ARE DISCLAIMED.  IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY
# DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
# DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS
# OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
# HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
# STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING
# IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
# POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleMesh.Service
{
    public class MType
    {
        public UInt16 TypeID;
        public string Name;
        public bool Static;
        public MType()
        {
            this.Static = false;
        }
    }
    public class TypeList
    {
        public TypeList()
        {
            this._ByID = new Dictionary<ushort, MType>();
            this._ByName = new Dictionary<string, MType>();
            MType scratch;
            scratch = new MType();
            scratch.Name = "Control.GetTypeID";
            scratch.TypeID = 57005;
            scratch.Static = true;
            this.Add(scratch);
            scratch = new MType();
            scratch.Name = "Control.TypeID";
            scratch.TypeID = 48879;
            scratch.Static = true;
            this.Add(scratch);
            scratch = new MType();
            scratch.Name = "Control.Auth.UUID";
            scratch.TypeID = 6500;
            this.Add(scratch);
            scratch = new MType();
            scratch.Name = "Control.Auth.Challenge";
            scratch.TypeID = 6501;
            this.Add(scratch);
            scratch = new MType();
            scratch.Name = "Control.Auth.Response";
            scratch.TypeID = 6502;
            this.Add(scratch);
            scratch = new MType();
            scratch.Name = "Control.Auth.OK";
            scratch.TypeID = 6503;
            this.Add(scratch);
            scratch = new MType();
            scratch.Name = "Control.Auth.Failed";
            scratch.TypeID = 6504;
            this.Add(scratch);
            scratch = new MType();
            scratch.Name = "Control.Auth.Types";
            scratch.TypeID = 6505;
            this.Add(scratch);
            scratch = new MType();
            scratch.Name = "Control.Ping";
            scratch.TypeID = 1;
            scratch.Static = true;
            this.Add(scratch);
            scratch = new MType();
            scratch.Name = "Control.Pong";
            scratch.TypeID = 2;
            scratch.Static = true;
            this.Add(scratch);
            /*
            scratch = new Type();
            scratch.Name = "Control.Auth.Types";
            scratch.TypeID = 6505;
            this.Add(scratch);
            */
        }
        private Dictionary<UInt16, MType> _ByID;
        private Dictionary<String, MType> _ByName;
        public Message Add(MType name) {
            _ByID.Add(name.TypeID, name);
            _ByName.Add(name.Name, name);
            return new Message();
        }
        public Message Remove(MType name)
        {
            return new Message();
        }
        public MType ByName(string name)
        {
            return _ByName[name];
        }
        public MType ByID(UInt16 id)
        {
            return _ByID[id];
        }
    }
}
