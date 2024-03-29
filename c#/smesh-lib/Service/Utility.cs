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
using Org.BouncyCastle.Crypto.Digests;
using System.Security.Cryptography;
using SimpleMesh;
using SimpleMesh.Service.AppProtocol;

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
    }
}
