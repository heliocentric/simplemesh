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
    public interface IMessage
    {
        UUID Remote
        {
            get;
            set;
        }
        UInt16 Conversation
        {
            get;
            set;
        }
        string Type
        {
            get;
            set;
        }
        UInt16 Sequence
        {
            get;
            set;
        }
        byte[] Payload
        {
            get;
            set;
        }
        UInt16 DataLength
        {
            get;
        }
        void FromMessage(IMessage msg);
    }

    public class Message : IMessage
    {
        private UUID _Remote;
        public UUID Remote
        {
            get { return _Remote; }
            set { _Remote = value; }
        }
        private UInt16 _Conversation;
        public UInt16 Conversation
        {
            get { return _Conversation; }
            set { _Conversation = value; }
        }
        private string _Type;
        public string Type
        {
            get { return _Type; }
            set { _Type = value; }
        }
        private UInt16 _Sequence;
        public UInt16 Sequence
        {
            get { return _Sequence; }
            set { _Sequence = value; }
        }

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
        public override string ToString()
        {
            return "Len=" + this.Payload.Length + " Con=" + this.Conversation + " Seq=" + this.Sequence + " Type=" + this.Type + " Payload=" + Encoding.UTF8.GetString(this.Payload);
        }
        public void FromMessage(IMessage msg)
        {
            this.Type = msg.Type;
            this.Remote = msg.Remote;
            this.Payload = msg.Payload;
            this.Sequence = msg.Sequence;
            this.Conversation = msg.Conversation;
        }
    }

    public class BinaryMessage : Message, IMessage
    {
    }

    public class TextMessage : IMessage
    {
        public TextMessage(string type)
        {
            this.Type = type;
        }
        public TextMessage(IMessage message)
        {
            this.FromMessage(message);
        }
        private UUID _remote;

        public UUID Remote
        {
            get { return _remote; }
            set { _remote = value; }
        }
        private string _type;

        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }
        private UInt16 _conversation;

        public UInt16 Conversation
        {
            get { return _conversation; }
            set { _conversation = value; }
        }
        private UInt16 _sequence;

        public UInt16 Sequence
        {
            get { return _sequence; }
            set { _sequence = value; }
        }
        private string _data;

        public string Data
        {
            get { return _data; }
            set { _data = value; }
        }
        public byte[] Payload
        {
            get
            {
                return Encoding.UTF8.GetBytes(this._data);
            }
            set
            {
                this._data = Encoding.UTF8.GetString(value);
            }
        }
        public UInt16 DataLength
        {
            get
            {
                return (UInt16) Encoding.UTF8.GetBytes(this._data).Length;
            }
        }
        public override string ToString()
        {
            return "Len=" + this.Payload.Length + " Con=" + this.Conversation + " Seq=" + this.Sequence + " Type=" + this.Type + " Payload=" + this.Data;

        }
        public void FromMessage(IMessage msg)
        {
            this.Type = msg.Type;
            this.Remote = msg.Remote;
            this.Payload = msg.Payload;
            this.Sequence = msg.Sequence;
            this.Conversation = msg.Conversation;
        }
    }


}
