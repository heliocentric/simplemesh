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
    /* Unix Style Timestamps */
    public class Time
    {
        public Time()
        {
            Construct(System.DateTime.Now);
        }
        public Time(System.DateTime dt) {
            Construct(dt);
        }
        public Time(string timeval)
        {
            string[] parts = timeval.Split(':');
            this.Major = Convert.ToInt64(parts[0]);
            if (parts.Length == 2)
            {
                this.Minor = Convert.ToInt64(parts[1]);
            }
            else
            {
                this.Minor = 0;
            }
        }
        public static Time operator +(Time time1, string time2)
        {
            return Add(time1, new Time(time2));
        }
        public static Time operator +(Time time1, Time time2)
        {
            return Add(time1, time2);
        }
        public static Time Add(Time time1, Time time2) {
            Time newtime = new Time();
            newtime.Minor = time1.Minor + time2.Minor;
            long majordelta = newtime.Minor / 10000000;
            long majorfractional = newtime.Minor % 10000000;
            if (majordelta >= 1)
            {
                newtime.Minor = majorfractional;
                newtime.Major = time1.Major + time2.Major + 1;
            }
            else
            {

                newtime.Major = time1.Major + time2.Major;
            }
            return newtime;
        }
        public static Time operator -(Time time1, string time2)
        {
            return Subtract(time1, new Time(time2));
        }
        public static Time operator -(Time time1, Time time2)
        {
            return Subtract(time1, time2);
        }
        public static Time Subtract(Time time1, Time time2)
        {
            Time newtime = new Time();
            long rtime1 = ((time1.Major * 10000000) + time1.Minor);
            long rtime2 = ((time2.Major * 10000000) + time2.Minor);
            long time = rtime1 - rtime2;
            newtime.Major = time / 10000000;
            newtime.Minor = time % 10000000;
            return newtime;
        }
        private void Construct(System.DateTime dt)
        {
            DateTime unixRef = new DateTime(1970, 1, 1, 0, 0, 0);
            Major = (dt.Ticks - unixRef.Ticks) / 10000000;
            Minor = (dt.Ticks - unixRef.Ticks) % 10000000;
        }
        public override string ToString()
        {
            return Major.ToString() + ":" + Minor.ToString();
        }
        long Major {
            get {
                return Seconds;
            }
            set {
                Seconds = value;
            }
        }
        long Minor {
            get {
                return USeconds;
            }
            set {
                USeconds = value;
            }
        }
        long Seconds;
        long USeconds;
    }
}
