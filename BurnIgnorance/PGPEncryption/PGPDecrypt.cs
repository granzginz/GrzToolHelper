using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Utilities.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GrzToolHelper.BurnIgnorance.PGPEncryption
{
    public class PGPDecrypt
    {
        public string _encryptedFilePath;
        public string _privKeyPath;
        public char[] _password;
        public string _outputPath;
        public PgpEncryptionKeys pgpKeys;
        public PGPDecrypt(string encryptedFilePath, string privKeyPath, string password, string outputPath, string pubKeyPath)
        {
            _encryptedFilePath = encryptedFilePath;
            _outputPath = outputPath;
            _password = password.ToCharArray();
            _privKeyPath = privKeyPath;
            pgpKeys = new PgpEncryptionKeys(pubKeyPath, privKeyPath, password);
        }
        public void decrypt(Stream input, string outputpath)
        {
            input = PgpUtilities.GetDecoderStream(input);
            try
            {
                PgpObjectFactory pgpObjF = new PgpObjectFactory(input);
                PgpEncryptedDataList enc;
                PgpObject obj = pgpObjF.NextPgpObject();
                if (obj is PgpEncryptedDataList)
                {
                    enc = (PgpEncryptedDataList)obj;
                }
                else
                {
                    enc = (PgpEncryptedDataList)pgpObjF.NextPgpObject();
                }
                PgpPrivateKey privKey = pgpKeys.PrivateKey;
                PgpPublicKeyEncryptedData pbe = null;
                foreach (PgpPublicKeyEncryptedData pked in enc.GetEncryptedDataObjects())
                {
                    if (privKey != null)
                    {
                        pbe = pked;
                        break;
                    }
                }
                Stream clear = pbe.GetDataStream(privKey);
                PgpObjectFactory plainFact = new PgpObjectFactory(clear);
                PgpObject message = plainFact.NextPgpObject();
                if (message is PgpCompressedData)
                {
                    PgpCompressedData cData = (PgpCompressedData)message;
                    Stream compDataIn = cData.GetDataStream();
                    PgpObjectFactory o = new PgpObjectFactory(compDataIn);
                    message = o.NextPgpObject();
                    if (message is PgpOnePassSignatureList)
                    {
                        message = o.NextPgpObject();
                        PgpLiteralData Ld = null;
                        Ld = (PgpLiteralData)message;
                        Stream output = File.Create(outputpath + "\\" + Ld.FileName);
                        Stream unc = Ld.GetInputStream();
                        Streams.PipeAll(unc, output);
                    }
                    else
                    {
                        PgpLiteralData Ld = null;
                        Ld = (PgpLiteralData)message;
                        Stream output = File.Create(outputpath + "\\" + Ld.FileName);
                        Stream unc = Ld.GetInputStream();
                        Streams.PipeAll(unc, output);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
