using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using PgpSharp;
using PgpSharp.GnuPG;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;


namespace GrzToolHelper.GNUPG
{
    public class PGCrypto
    {
        IPgpTool tool = new GnuPGTool();
        public PGCrypto() {

        }

        public string EncryptGnuPG(string inputpPathFile, string outputPathFile, string recipient)
        {
           // string a = "";

            if (checkPathGnuPg())
            {
                

                var encryptArg = new FileDataInput
                {
                    Armor = true,
                    InputFile = inputpPathFile,
                    OutputFile = outputPathFile,
                    Operation = DataOperation.Encrypt,
                    Recipient = recipient,
                };

                try
                {
                    tool.ProcessData(encryptArg);
                }
                catch (Exception e)
                {
                    return "encrypt failed " + e.Message;
                    throw;
                }
                

            }

            return "encrypt finished check file";
        }


        public string DecryptGnuPG(string inputpPathFile, string outputPathFile, SecureString yourPassphrase)
        {
           // string a = "";

            if (checkPathGnuPg())
            {
               
                var decryptArg = new FileDataInput
                {
                    InputFile = inputpPathFile,
                    OutputFile = outputPathFile,
                    Operation = DataOperation.Decrypt,
                    Passphrase = yourPassphrase
                };

                try
                {
                    tool.ProcessData(decryptArg);
                }
                catch (Exception e)
                {
                    return "decrypt failed " + e.Message;
                    throw;
                }

                
            }

               

            return "decrypt finished check file";
        }

        public string ListKey()
        {
            return tool.ListKeys(KeyTarget.Public).ToList<KeyId>().FirstOrDefault();
        }

        public bool checkPathGnuPg()
        {

            try
            {
                GnuPGConfig.GnuPGExePath = @"C:\Program Files (x86)\GnuPG\bin\gpg.exe";
            }
            catch (Exception)
            {
                return false;
                throw;
            }

            return true;
        }


        //public string RSAPrivateKey()
        //{

        //    AsymmetricCipherKeyPair ackp = new AsymmetricCipherKeyPair();


        //    return "";
        //}

        //public string RSAPublicKey()
        //{

        //    return "";
        //}


        /// <summary>
        /// Build a PGP key pair
        /// </summary>
        /// <param name="bits">number of bits in key, e.g. 2048</param>
        /// <param name="identifier">key identifier, e.g. "Your Name <your@emailaddress.com>" </param>
        /// <param name="password">key password or null</param>
        /// <param name="privateKey">returned ascii private key</param>
        /// <param name="publicKey">returned ascii public key</param>
        public void PGPGenerateKey(int bits, string identifier, string password, out string privateKey, out string publicKey)
        {
            // generate a new RSA keypair 
            RsaKeyPairGenerator gen = new RsaKeyPairGenerator();
            gen.Init(new RsaKeyGenerationParameters(BigInteger.ValueOf(0x101), new Org.BouncyCastle.Security.SecureRandom(), bits, 80));
            AsymmetricCipherKeyPair pair = gen.GenerateKeyPair();

            // create PGP subpacket
            PgpSignatureSubpacketGenerator hashedGen = new PgpSignatureSubpacketGenerator();
            hashedGen.SetKeyFlags(true, PgpKeyFlags.CanCertify | PgpKeyFlags.CanSign | PgpKeyFlags.CanEncryptCommunications | PgpKeyFlags.CanEncryptStorage);
            hashedGen.SetPreferredCompressionAlgorithms(false, new int[] { (int)CompressionAlgorithmTag.Zip });
            hashedGen.SetPreferredHashAlgorithms(false, new int[] { (int)HashAlgorithmTag.Sha1 });
            hashedGen.SetPreferredSymmetricAlgorithms(false, new int[] { (int)SymmetricKeyAlgorithmTag.Cast5 });
            PgpSignatureSubpacketVector sv = hashedGen.Generate();
            PgpSignatureSubpacketGenerator unhashedGen = new PgpSignatureSubpacketGenerator();

            // create the PGP key
            PgpSecretKey secretKey = new PgpSecretKey(
              PgpSignature.DefaultCertification,
              PublicKeyAlgorithmTag.RsaGeneral,
              pair.Public,
              pair.Private,
              DateTime.Now,
              identifier,
              SymmetricKeyAlgorithmTag.Cast5,
              (password != null ? password.ToCharArray() : null),
              hashedGen.Generate(),
              unhashedGen.Generate(),
              new Org.BouncyCastle.Security.SecureRandom());

            // extract the keys
            using (MemoryStream ms = new MemoryStream())
            {
                using (ArmoredOutputStream ars = new ArmoredOutputStream(ms))
                {
                    secretKey.Encode(ars);
                }
                privateKey = Encoding.ASCII.GetString(ms.ToArray());
            }
            using (MemoryStream ms = new MemoryStream())
            {
                using (ArmoredOutputStream ars = new ArmoredOutputStream(ms))
                {
                    secretKey.PublicKey.Encode(ars);
                }
                publicKey = Encoding.ASCII.GetString(ms.ToArray());
            }
        }


        public void SignatureGenerator()
        {
            

        }
    }
}
