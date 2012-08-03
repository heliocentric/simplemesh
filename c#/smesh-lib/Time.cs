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
