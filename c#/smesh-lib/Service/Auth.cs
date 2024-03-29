﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleMesh.Service
{
    public class Auth
    {
        public Key Key;
        public UUID UUID;
        public Boolean Active;
        public Boolean Primary;
        public Auth()
        {
        }
        public Auth(string keystring)
        {

            this.Key = new Key(keystring);
            this.Active = true;
            this.Primary = false;
            this.UUID = this.Key.UUID;
        }
        public Auth(string keystring, Boolean active, Boolean primary)
        {
            this.Key = new Key(keystring);
            this.Active = active;
            this.Primary = primary;
            this.UUID = this.Key.UUID;
        }
        public override string ToString()
        {
            return this.Key.ToString();
        }
        public override int GetHashCode()
        {
            return this.UUID.GetHashCode();
        }
    }
}
