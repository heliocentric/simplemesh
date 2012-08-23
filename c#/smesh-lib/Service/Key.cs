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
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Encodings;

namespace SimpleMesh.Service
{
    public class Key
    {
        public UUID UUID;
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
            this.UUID = new UUID();
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
                    retval = this.UUID.ToString() + "!RSA!" + this.Length.ToString() + "!" + derPublicKey + "!" + derPrivateKey;
                    break;
                default:
                    retval = this.UUID.ToString() + "!NONE!!!";
                    break;
            }
            return retval;
        }
        public void Decode(string key) {
            string [] chunks = key.Split('!');
            this.UUID = new UUID(chunks[0]);
            this.Type = chunks[1];
            switch (this.Type)
            {
                case "RSA":
                    if (chunks[2] != "")
                    {
                        this.Length = Convert.ToInt32(chunks[2]);
                    }
                    
                    string derPublicKey = chunks[3];
                    string derPrivateKey = chunks[4];


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

        public IMessage Encrypt(Boolean normal, byte[] data, out string outdata)
        {
            byte[] ciphertext;
            IMessage msg = this.Encrypt(normal, data, out ciphertext);
            outdata = System.Convert.ToBase64String(ciphertext);
            return msg;
        }
        public IMessage Decrypt(Boolean normal, string data, out byte[] outdata)
        {
            byte[] ciphertext = System.Convert.FromBase64String(data.Replace("\0", string.Empty));
            IMessage msg = this.Decrypt(normal, ciphertext, out outdata);
            return msg;
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
                        SecureRandom rand = new SecureRandom();
                        IBufferedCipher cipher = CipherUtilities.GetCipher("RSA/NONE/OAEPWithSHA1AndMGF1Padding");
                        if (normal == true)
                        {
                            cipher.Init(true, new ParametersWithRandom(this.PublicKey, rand));
                        }
                        else
                        {
                            cipher.Init(true, new ParametersWithRandom(this.PrivateKey, rand));
                        }

                        byte[] cipherTextBlock = null;
                        int outputsize = cipher.GetOutputSize(data.Length); //Array size of ciphered data (encrypted data)
                        int blockSize = cipher.GetBlockSize();  //Amount of data we can process at one time (-2 -2*hlen)
                        List<byte> output = new List<byte>();
                        int outputLen = 0;
                        byte[] dataToProcess = null;
                        for (int chunkPosition = 0; chunkPosition < data.Length; chunkPosition += blockSize)
                        {
                            dataToProcess = new byte[blockSize];
                            int chunkSize = (data.Length - chunkPosition) < blockSize ? (data.Length - chunkPosition) : blockSize; //Math.Min(blockSize, data.Length - (chunkPosition * blockSize));
                            Buffer.BlockCopy(data, chunkPosition, dataToProcess, 0, chunkSize);



                            cipherTextBlock = new byte[outputsize];



                            outputLen = cipher.ProcessBytes(dataToProcess, 0, chunkSize, cipherTextBlock, 0);
                            cipher.DoFinal(cipherTextBlock, outputLen);
                            //output.AddRange(e.ProcessBytes(data, chunkPosition,
                            //  chunkSize));
                            output.AddRange(cipherTextBlock);
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
                        SecureRandom rand = new SecureRandom();
                        IBufferedCipher cipher = CipherUtilities.GetCipher("RSA/NONE/OAEPWithSHA1AndMGF1Padding");


                        if (normal == true)
                        {
                            cipher.Init(false, new ParametersWithRandom(this.PrivateKey, rand));
                        }
                        else
                        {

                            cipher.Init(false, new ParametersWithRandom(this.PublicKey, rand));
                        }



                        byte[] cipherTextBlock = null;
                        int outputsize = cipher.GetOutputSize(data.Length); //Array size of ciphered data (encrypted data)
                        int blockSize = cipher.GetBlockSize();  //Amount of data we can process at one time (-2 -2*hlen)
                        List<byte> output = new List<byte>();
                        int outputLen = 0;
                        byte[] dataToProcess = null;



                        for (int chunkPosition = 0; chunkPosition < data.Length; chunkPosition += blockSize)
                        {
                            dataToProcess = new byte[blockSize];
                            int chunkSize = (data.Length - chunkPosition) < blockSize ? (data.Length - chunkPosition) : blockSize; //Math.Min(blockSize, data.Length - (chunkPosition * blockSize));
                            Buffer.BlockCopy(data, chunkPosition, dataToProcess, 0, chunkSize);



                            cipherTextBlock = new byte[outputsize];



                            outputLen = cipher.ProcessBytes(dataToProcess, 0, chunkSize, cipherTextBlock, 0);
                            cipher.DoFinal(cipherTextBlock, outputLen);
                            //output.AddRange(e.ProcessBytes(data, chunkPosition,
                            //  chunkSize));
                            output.AddRange(cipherTextBlock);
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
