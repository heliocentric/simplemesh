/*
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

namespace SimpleMesh
{
    public class Message
    {
        public UUID Remote;
        
        public UInt16 Conversation;
        public string Type;
        public UInt16 Sequence;

        private byte[] _payload;
        public virtual byte[] Payload
        {
            get { return _payload; }
            set { _payload = value; }
        }
        public Message()
        {
            this.Constructor("");
        }
        public Message(string type)
        {
            this.Constructor(type);
        }
        protected void Constructor(string type)
        {
            this.Type = type;
            this.Payload = new byte[0];
            this.Conversation = 0;
            this.Sequence = 0;
        }
        public virtual UInt16 DataLength {
            get {
                return (UInt16) this.Payload.Length;
            }
        }
        public virtual void Pack()
        {
        }
        public virtual void Unpack()
        {
        }
    }
    public class BinaryMessage : Message
    {
        public BinaryMessage(string type)
            : base(type)
        {
        }
        public byte[] Data;
        public override ushort DataLength
        {
            get
            {
                return base.DataLength;
            }
        }
        public override void Pack()
        {
            this.Payload = this.Data;
        }
        public override void Unpack()
        {
            this.Data = this.Payload;
        }
    }
    public class TextMessage : Message
    {
        public TextMessage(string type)
            : base(type)
        {
        }
        public string Data {
        get {
            return Encoding.UTF8.GetString(this.Payload);
        }
            set {
                this.Payload = Encoding.UTF8.GetBytes(value);
            }
        }
        public override byte[] Payload
        {
            get
            {
                return base.Payload;
            }
            set
            {
                base.Payload = value;
            }
        }
        public override void Pack()
        {
            this.Payload = Encoding.UTF8.GetBytes(this.Data);
        }
        public override void Unpack()
        {
            this.Data = Encoding.UTF8.GetString(this.Payload);
        }
    }
}
