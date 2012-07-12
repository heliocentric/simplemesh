using System;
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
                    if (this.PrivateKey != null)
                    {
                        PrivateKeyInfo infoPrivate = PrivateKeyInfoFactory.CreatePrivateKeyInfo(this.PrivateKey);
                        derPrivateKey = Convert.ToBase64String(infoPrivate.GetEncoded());
                    }
                    if (this.PublicKey != null)
                    {
                        SubjectPublicKeyInfo publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(this.PublicKey);
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

                    this.PublicKey = PublicKeyFactory.CreateKey(publicKeyBytes);
                    this.PrivateKey = PrivateKeyFactory.CreateKey(privateKeyBytes);
                    break;
            }
        }
        public override string ToString()
        {
            return this.Encode();
        }
    }
}
