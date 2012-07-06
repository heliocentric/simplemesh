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

namespace SimpleMesh.Service
{
    static public class Utility
    {
        public static byte[] MessagePack(Message message, TypeList lookuptable)
        {
            byte[] header = new byte[8];
            byte[] bytescratch;
            bytescratch = SimpleMesh.Utility.ToNetworkOrder(message.DataLength);
            header[0] = bytescratch[0];
            header[1] = bytescratch[1];
            bytescratch = SimpleMesh.Utility.ToNetworkOrder(message.Conversation);
            header[2] = bytescratch[0];
            header[3] = bytescratch[1];
            bytescratch = SimpleMesh.Utility.ToNetworkOrder(lookuptable.ByName(message.Type).TypeID);
            header[4] = bytescratch[0];
            header[5] = bytescratch[1];
            bytescratch = SimpleMesh.Utility.ToNetworkOrder(message.Sequence);
            header[6] = bytescratch[0];
            header[7] = bytescratch[1];
            byte[] rv = new byte[header.Length + message.Payload.Length];
            System.Buffer.BlockCopy(header, 0, rv, 0, header.Length);
            System.Buffer.BlockCopy(message.Payload, 0, rv, header.Length, message.Payload.Length);
            return rv;
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

    }
}
