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
using Org.BouncyCastle.Crypto.Digests;
using System.Security.Cryptography;

namespace SimpleMesh.Service
{
    static public class Utility
    {
        public static String MD5(String toHashMD5)
        {
            MD5Digest digest = new MD5Digest();
            byte[] scratch = new byte[digest.GetDigestSize()];
            digest.BlockUpdate(UTF8Encoding.UTF8.GetBytes(toHashMD5), 0, UTF8Encoding.UTF8.GetByteCount(toHashMD5));
            digest.DoFinal(scratch, 0);

            string hex = BitConverter.ToString(scratch).ToLower();
            return hex.Replace("-", "");
        }
        public static String SHA256(string tosha256)
        {
            Sha256Digest digest = new Sha256Digest();
            byte[] scratch = new byte[digest.GetDigestSize()];
            digest.BlockUpdate(UTF8Encoding.UTF8.GetBytes(tosha256), 0, UTF8Encoding.UTF8.GetByteCount(tosha256));
            digest.DoFinal(scratch, 0);

            string hex = BitConverter.ToString(scratch).ToLower();
            return hex.Replace("-", "");
        }

        public static long ToUnixTimestamp(System.DateTime dt)
        {
            DateTime unixRef = new DateTime(1970, 1, 1, 0, 0, 0);
            return (dt.Ticks - unixRef.Ticks) / 10000000;
        }
        public static DateTime FromUnixTimestamp(long timestamp)
        {
            DateTime unixRef = new DateTime(1970, 1, 1, 0, 0, 0);
            return unixRef.AddSeconds(timestamp);
        }
        public static string Version
        {
            get
            {
                return Properties.Resources.ServiceVersion;
            }
        }
        public static byte[] MessagePack(Message message, MessageBrokerArgs args)
        {
            byte[] rv = new byte[1];
            rv[0] = 0xFF;
            switch (args.Connector.AppProtocol)
            {
                case "native1":
                    byte[] header = new byte[8];
                    byte[] bytescratch;
                    bytescratch = SimpleMesh.Utility.ToNetworkOrder(message.DataLength);
                    header[0] = bytescratch[0];
                    header[1] = bytescratch[1];
                    bytescratch = SimpleMesh.Utility.ToNetworkOrder(message.Conversation);
                    header[2] = bytescratch[0];
                    header[3] = bytescratch[1];
                    bytescratch = SimpleMesh.Utility.ToNetworkOrder(args.Types.ByName(message.Type).TypeID);
                    header[4] = bytescratch[0];
                    header[5] = bytescratch[1];
                    bytescratch = SimpleMesh.Utility.ToNetworkOrder(message.Sequence);
                    header[6] = bytescratch[0];
                    header[7] = bytescratch[1];
                    rv = new byte[header.Length + message.Payload.Length];
                    System.Buffer.BlockCopy(header, 0, rv, 0, header.Length);
                    System.Buffer.BlockCopy(message.Payload, 0, rv, header.Length, message.Payload.Length);
                    break;
            }
            return rv;
        }
        public static int SendMessage(MessageBrokerArgs args, Message msg)
        {
            int retval = 1;
            byte[] packed;
            packed = Utility.MessagePack(msg, args);
            switch(args.Connector.Protocol) {
                case "tcp":
                    Runner.DebugMessage("Debug.Info.Message", "T: " + msg.ToString());
                    args.Socket.Send(packed);
                    break;
            }
            return retval;
        }
        public static Message RecieveMessage(MessageBrokerArgs args)
        {
            Message retval = new Message();
            byte [] header = new byte[8];
            int count;
            try
            {
                count = args.Socket.Receive(header);
            }
            catch
            {
                retval.Type = "Error.Message.Corrupted";
                return retval;
            }
            if (count == 8)
            {
                byte[] _length = new byte[2];
                byte[] _conversation = new byte[2];
                byte[] _typeid = new byte[2];
                byte[] _sequence = new byte[2];

                _length[0] = header[0];
                _length[1] = header[1];
                _conversation[0] = header[2];
                _conversation[1] = header[3];
                _typeid[0] = header[4];
                _typeid[1] = header[5];
                _sequence[0] = header[6];
                _sequence[1] = header[7];


                UInt16 plength;
                UInt16 pconversation;
                UInt16 ptypeid;
                UInt16 psequence;

                plength = SimpleMesh.Utility.ToHostOrder(_length);
                pconversation = SimpleMesh.Utility.ToHostOrder(_conversation);
                ptypeid = SimpleMesh.Utility.ToHostOrder(_typeid);
                psequence = SimpleMesh.Utility.ToHostOrder(_sequence);

                byte[] payload = new byte[plength];
                try
                {
                    count = args.Socket.Receive(payload);
                }
                catch
                {
                    retval.Type = "Error.Message.Corrupted";
                    return retval;
                }
                if (count == plength)
                {
                    retval.Sequence = psequence;
                    retval.Conversation = pconversation;
                    retval.Type = args.Types.ByID(ptypeid).Name;
                    retval.Payload = payload;
                }
                else
                {
                    retval.Type = "Error.Message.Corrupted";
                    return retval;
                }
            }
            else
            {
                retval.Type = "Error.Message.Corrupted";
            }
            return retval;

        }
    }
}
