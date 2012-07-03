using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public Key() {
        }
        public string Encode() {
            string derPublicKey;
            string derPrivateKey;
            string retval;
            switch (this.Type)
            {
                case "RSA":
                    if (BouncyPair.Public != null)
                    {
                        PrivateKeyInfo infoPrivate = PrivateKeyInfoFactory.CreatePrivateKeyInfo(this.BouncyPair.Private);
                        byte[] serializedPrivateKey = infoPrivate.PrivateKey.ToAsn1Object().GetDerEncoded();
                        derPrivateKey = Convert.ToBase64String(serializedPrivateKey);
                    }
                    else
                    {
                        derPrivateKey = "";
                    }
                    if (BouncyPair.Public != null)
                    {
                        SubjectPublicKeyInfo publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(this.BouncyPair.Public);
                        byte[] serializedPublicKey = publicKeyInfo.PublicKeyData.ToAsn1Object().GetDerEncoded();
                        derPublicKey = Convert.ToBase64String(serializedPublicKey);
                    }
                    else
                    {
                        derPublicKey = "";
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
            if (chunks[1] != "")
            {
                this.Length = Convert.ToInt32(chunks[1]);
            }
            string derPublicKey = chunks[2];
            string derPrivateKey = chunks[3];

        }
        public override string ToString()
        {
            return this.Encode();
        }
    }
}
