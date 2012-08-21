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
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Encodings;

namespace SimpleMesh.Service
{
    public class Key
    {
        public int Length;
        public string Type;
        private AsymmetricKeyParameter _PublicKey;

        public AsymmetricKeyParameter PublicKey
        {
            get { return _PublicKey; }
            set
            {
                this._containspublickey = true;
                _PublicKey = value;
            }
        }
        private AsymmetricKeyParameter _PrivateKey;

        public AsymmetricKeyParameter PrivateKey
        {
            get { return _PrivateKey; }
            set
            {
                this._containsprivatekey = true;
                _PrivateKey = value;
            }
        }
        private bool _containsprivatekey;
        private bool _containspublickey;
        public string Salt;
        public string Hash;

        public Key() {
            this._containsprivatekey = false;
            this._containspublickey = false;
        }
        
        public Key(string key)
        {
            this.Decode(key);
        }
        public string Encode()
        {
            return this.Encode(true, true);
        }
        public string Encode(bool Private)
        {
            return this.Encode(Private, true);
        }
        public string Encode(bool Private, bool Public) {
            string derPublicKey;
            string derPrivateKey;
            string retval;

            derPrivateKey = "";
            derPublicKey = "";
            switch (this.Type)
            {
                case "RSA":
                    if (this._containsprivatekey == true && Private == true)
                    {
                        PrivateKeyInfo infoPrivate = PrivateKeyInfoFactory.CreatePrivateKeyInfo(this.PrivateKey);
                        derPrivateKey = Convert.ToBase64String(infoPrivate.GetEncoded());
                    }
                    if (this._containspublickey = true && Public == true)
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


                    if (derPublicKey != "")
                    {
                        try
                        {
                            byte[] publicKeyBytes = Convert.FromBase64String(derPublicKey);
                            this.PublicKey = PublicKeyFactory.CreateKey(publicKeyBytes);
                            this._containspublickey = true;
                        }
                        catch
                        {
                            this._containspublickey = false;
                        }
                    }
                    if (derPrivateKey != "")
                    {
                        try
                        {
                            byte[] privateKeyBytes = Convert.FromBase64String(derPrivateKey);
                            this.PrivateKey = PrivateKeyFactory.CreateKey(privateKeyBytes);
                            this._containsprivatekey = true;
                        }
                        catch
                        {
                            this._containsprivatekey = false;
                        }
                    }
                    break;
            }
        }
        public override string ToString()
        {
            return this.Encode();
        }
        public int Generate()
        {
            return this.Generate("RSA", 4096);
        }
        public int Generate(string Type)
        {
            int retval;
            retval = 2;
            switch (Type)
            {
                case "RSA":
                    retval = this.Generate(Type, 4096);
                    break;
                case "HASH":
                    retval =  1;
                    break;
            }
            return retval;
        }
        /* Option is the password string if it is a hash, and the key length if it is RSA */
        public int Generate(string Type, string Option)
        {
            int retval;
            retval = 2;
            switch (Type)
            {
                case "RSA":
                    try
                    {
                        int value = Convert.ToInt32(Option);
                        retval = this.Generate(Type, value);
                    }
                    catch
                    {
                        retval = this.Generate(Type, 4096);
                    }
                    break;
                case "HASH":
                    retval = this.Generate(Type, "SHA256", Option);
                    break;
            }
            return retval;
        }
        /* Only use for Asymmetric Keys */
        public int Generate(string Type, int Keylength)
        {
            int retval;
            retval = 2;
            switch (Type)
            {
                case "RSA":
                    this.Type = "RSA";
                    this.Length = Keylength;
                    SimpleMesh.Service.Runner.DebugMessage("Debug.Info.KeyGen", "Generating " + this.Length.ToString() + " bit RSA key pair starting on: " + DateTime.Now.ToString());
                    RsaKeyPairGenerator r = new RsaKeyPairGenerator();
                    r.Init(new KeyGenerationParameters(new SecureRandom(), this.Length));
                    AsymmetricCipherKeyPair test = r.GenerateKeyPair();
                    this.PrivateKey = test.Private;
                    this.PublicKey = test.Public;
                    this._containsprivatekey = true;
                    this._containspublickey = true;
                    break;
                case "HASH":
                    retval = 1;
                    break;
            }
            return retval;
        }
        /* Only use for HASH */
        public int Generate(string Type, string HashAlgorithm, string key)
        
        {
            int retval = 2;
            switch (Type)
            {
                case "RSA":
                    retval = 1;
                    break;
                case "HASH":
                    break;
            }
            return retval;
        }

        public IMessage Encrypt(Boolean normal, byte[] data, out byte[] outdata)
        {
            
            IMessage msg = new TextMessage("Error.Unknown");
            outdata = new byte[1];
            switch (this.Type)
            {
                case "RSA":
                    try
                    {
                        IAsymmetricBlockCipher cipher = new RsaEngine();
                        if (normal == true)
                        {
                            cipher.Init(true, this.PublicKey);
                        }
                        else
                        {
                            cipher.Init(true, this.PrivateKey);
                        }
                        int blocksize = cipher.GetInputBlockSize();
                        List<byte> output = new List<byte>();
                        for (int chunkPosition = 0; chunkPosition < data.Length; chunkPosition += blocksize)
                        {
                            int chunkSize = Math.Min(blocksize, data.Length - (chunkPosition * blocksize));
                            output.AddRange(cipher.ProcessBlock(data, chunkPosition, chunkSize));
                        }
                        outdata = output.ToArray();

                        msg.Type = "Error.OK";
                    }
                    catch
                    {
                    }
                    break;
            }
            return msg;
        }
        public IMessage Decrypt(Boolean normal, byte[] data, out byte[] outdata)
        {
            IMessage msg = new TextMessage("Error.Unknown");
            outdata = new byte[1];
            switch (this.Type)
            {
                case "RSA":
                    try
                    {
                        IAsymmetricBlockCipher cipher = new RsaEngine();
                        if (normal == true)
                        {
                            cipher.Init(false, this.PrivateKey);
                        }
                        else
                        {
                            cipher.Init(false, this.PublicKey);
                        }
                        int blocksize = cipher.GetInputBlockSize();
                        List<byte> output = new List<byte>();
                        for (int chunkposition = 0; chunkposition < data.Length; chunkposition += blocksize)
                        {
                            int chunksize = Math.Min(blocksize, data.Length - (chunkposition * blocksize));
                            output.AddRange(cipher.ProcessBlock(data, chunkposition, chunksize));
                        }
                        outdata = output.ToArray();

                        msg.Type = "Error.OK";

                    }
                    catch
                    {
                        outdata = new byte[1];
                    }
                    break;
            }
            return msg;
        }

    }
}
