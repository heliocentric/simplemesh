﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO.Pem;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;

namespace SimpleMesh.Service
{
    public class Key
    {
        public int Length;
        public string Type;
        private AsymmetricCipherKeyPair _BouncyPair;

        public AsymmetricCipherKeyPair BouncyPair
        {
            get { return _BouncyPair; }
            set { _BouncyPair = value; }
        }
        public AsymmetricKeyParameter PublicKey;
        public AsymmetricKeyParameter PrivateKey;
        public string Salt;
        public string Hash;
        public Key() {
        }
        public Key(string key)
        {
            this.Decode(key);
        }

        public string Encode() {
            string derPublicKey;
            string derPrivateKey;
            string retval;

            derPrivateKey = "";
            derPublicKey = "";
            switch (this.Type)
            {
                case "RSA":
                    if (this.BouncyPair != null)
                    {
                        PrivateKeyInfo infoPrivate = PrivateKeyInfoFactory.CreatePrivateKeyInfo(this.BouncyPair.Private);
                        derPrivateKey = Convert.ToBase64String(infoPrivate.GetEncoded());
                        SubjectPublicKeyInfo publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(this.BouncyPair.Public);
                        derPublicKey = Convert.ToBase64String(publicKeyInfo.GetEncoded());
                    }
                    retval = "RSA!" + this.Length.ToString() + "!" + derPublicKey + "!" + derPrivateKey;
                    break;
                default:
                    retval = "NONE!!!";
                    break;
        }
            return retval;
        }
        public void Decode(string key) {
            string [] chunks = key.Split('!');
            this.Type = chunks[0];
            switch (this.Type)
            {
                case "RSA":
                    if (chunks[1] != "")
                    {
                        this.Length = Convert.ToInt32(chunks[1]);
                    }
                    
                    string derPublicKey = chunks[2];
                    string derPrivateKey = chunks[3];

                    byte[] publicKeyBytes = Convert.FromBase64String(derPublicKey);
                    byte[] privateKeyBytes = Convert.FromBase64String(derPrivateKey);
                    Encoding enc = new UTF8Encoding();
                    SimpleMesh.Service.Runner.DebugMessage("Debug.Info.ConfigFile", enc.GetString(publicKeyBytes));

                    AsymmetricKeyParameter publickeyparameter = PublicKeyFactory.CreateKey(publicKeyBytes);
                    AsymmetricKeyParameter privatekeyparameter = PrivateKeyFactory.CreateKey(privateKeyBytes);
                    this.BouncyPair = new AsymmetricCipherKeyPair(publickeyparameter, privatekeyparameter);
                    break;
            }
        }
        public override string ToString()
        {
            return this.Encode();
        }
    }
}
