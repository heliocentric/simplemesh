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

    public class Message : StubMessage, IMessage
    {
        private byte[] _payload;
        public override byte[] Payload
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
    }

    public class BinaryMessage : Message, IMessage
    {
    }

    public class TextMessage : StubMessage, IMessage
    {
        public TextMessage(string type)
        {
            this.Type = type;
        }
        public TextMessage(IMessage message)
        {
            this.FromMessage(message);
        }
        private string _data;

        public string Data
        {
            get { return _data; }
            set { _data = value; }
        }
        public override byte[] Payload
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
        public override string ToString()
        {
            return "Len=" + this.Payload.Length + " Con=" + this.Conversation + " Seq=" + this.Sequence + " Type=" + this.Type + " Payload=" + this._data;

        }

    }
    public class StubMessage
    {

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
        public override string ToString()
        {
            return "Len=" + this.Payload.Length + " Con=" + this.Conversation + " Seq=" + this.Sequence + " Type=" + this.Type + " Payload=0x" + BitConverter.ToString(this.Payload).Replace("-", string.Empty);

        }
        public void FromMessage(IMessage msg)
        {
            this.Type = msg.Type;
            this.Remote = msg.Remote;
            this.Payload = msg.Payload;
            this.Sequence = msg.Sequence;
            this.Conversation = msg.Conversation;
        }
        public virtual byte[] Payload
        {
            get;
            set;
        }
        public UInt16 DataLength
        {
            get
            {
                return (UInt16) this.Payload.Length;
            }
        }
    }
    public class KeyValueMessage : StubMessage, IMessage
    {
        public KeyValueMessage()
        {
            this.Constructor();
        }
        public KeyValueMessage(IMessage msg)
        {
            this.Constructor();
            this.FromMessage(msg);
        }
        private void Constructor()
        {
            this.TotalSize = 0;
            this.Data = new Dictionary<string, string>();
        }
        public Dictionary<string, string> Data;
        public int Add(string key, string value)
        {
            int retval = 1;
            string tvalue;
            if (this.Data.TryGetValue(key, out tvalue))
            {
                lock (this.Data)
                {
                    int oldvallen;
                    int newvallen;
                    oldvallen = Encoding.UTF8.GetBytes(this.Data[key]).Length;
                    newvallen = Encoding.UTF8.GetBytes(value).Length;
                    if (((TotalSize - oldvallen) + newvallen) < 65527)
                    {
                        this.Data[key] = value;
                        this.TotalSize = (ushort) ((this.TotalSize - oldvallen) + newvallen);
                    }
                    else
                    {
                        retval = 3;
                    }
                }
            }
            else
            {
                lock (this.Data)
                {
                    int chunklen = Encoding.UTF8.GetBytes(key).Length + Encoding.UTF8.GetBytes(value).Length + 4;
                    int newsize = this.TotalSize + chunklen;
                    if (newsize <= 65527)
                    {
                        this.Data.Add(key, value);
                        this.TotalSize = (ushort) newsize;
                    }
                    else
                    {
                        retval = 3;
                    }
                }
            }
            return retval;
        }
        public bool Remove(string key)
        {
            bool retval;
            lock (this.Data)
            {
                retval = this.Data.Remove(key);
                int tsize = this.TotalSize - Encoding.UTF8.GetBytes(key).Length - Encoding.UTF8.GetBytes(key).Length - 4;
                this.TotalSize = (ushort)tsize;
            }
            return retval;
        }
        private ushort _totalsize;

        public ushort TotalSize
        {
            get { return _totalsize; }
            set { _totalsize = value; }
        }

        public override byte[] Payload
        {
            get
            {
                byte [] retval;
                retval = new byte[0];
                return retval;
            }
            set
            {
                this.Constructor();
            }
        }
    }
}
