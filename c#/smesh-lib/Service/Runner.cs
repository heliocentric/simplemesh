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
    public class Runner
    {
        string _DatabasePath;
        string _StorePath;
        Trackfile Network;
        public string DatabasePath
        {
            get { return _DatabasePath; }
            set { _DatabasePath = value; }
        }
        public string StorePath
        {
            get { return _StorePath; }
            set { _StorePath = value; }
        }
        private string _privatekeyfile;
        private string _publickeyfile;
        public void Start()
        {

            this.StorePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData) + @"\NiftyEngineering";
            this._privatekeyfile = this.StorePath + @"\private.pem";
            this._publickeyfile = this.StorePath + @"\public.pem";
            Utility.DebugMessage(10, this.StorePath);
            if (System.IO.Directory.Exists(this.StorePath) == false)
            {
                System.IO.Directory.CreateDirectory(this.StorePath);
            }
            this.Network = new Trackfile();
            string TrackfilePath = this.StorePath + @"\default.tkf";
            if (System.IO.File.Exists(TrackfilePath) == true)
            {
                this.Network.Read(TrackfilePath);
            }
            else
            {

            }
            this.Network.Write(TrackfilePath);

            /*
            SimpleMesh.Service.Net.TCP scratch = new SimpleMesh.Service.Net.TCP();
            scratch.Listen();
             */
        }
    }
}
