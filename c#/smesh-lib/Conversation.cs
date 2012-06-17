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
    public class Conversation
    {
        public Conversation()
        {
        }
        public Conversation(string Token)
        {
            this.Token = Token;
        }
        public Conversation(string Token, UUID Remote)
        {
            this.Token = Token;
            this.Remote = Remote;
        }
        private UUID _remote;

        public UUID Remote
        {
            get { return _remote; }
            set { _remote = value; }
        }

        private string _token;

        public string Token
        {
            get { return _token; }
            set { _token = value; }
        }
        public void Send(Message scratch) {
        }
        public Message Recieve()
        {
            System.Threading.Thread.Sleep(10 * 100);
            return new Message();
        }
    }
}
