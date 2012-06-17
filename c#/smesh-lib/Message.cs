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
