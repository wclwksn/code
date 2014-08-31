using ScriptCoreLib;
using ScriptCoreLib.Delegates;
using ScriptCoreLib.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using java.security;
using javax.crypto;

namespace TestCryptoKeyGenerate
{
    //using EncryptedBytes = Byte[];
    // add CallSite? or callback delegate to undo?
    public class EncryptedBytes(public byte[] bytes) { }

    public class ApplicationWebService
    {
        // https://sites.google.com/a/jsc-solutions.net/backlog/knowledge-base/2014/201408/20140830
        // X:\jsc.svn\examples\java\hybrid\JVMCLRCryptoKeyGenerate\JVMCLRCryptoKeyGenerate\Program.cs
        // X:\jsc.svn\examples\javascript\android\Test\TestAndroidCryptoKeyGenerate\TestAndroidCryptoKeyGenerate\ApplicationWebService.cs
        // X:\jsc.svn\examples\javascript\appengine\Test\TestCryptoKeyGenerate\TestCryptoKeyGenerate\ApplicationWebService.cs

        private static readonly KeyPair keyPair;

        static ApplicationWebService()
        {
            try
            {
                // it works.
                // can we now wrap rsa for all platforms
                // and use it as a generic nuget?

                var sw = Stopwatch.StartNew();
                Console.WriteLine("before generateKeyPair " + new { sw.ElapsedMilliseconds });

                var keyGen = KeyPairGenerator.getInstance("RSA");

                keyGen.initialize(2048);

                keyPair = keyGen.generateKeyPair();
                Console.WriteLine("after generateKeyPair " + new { sw.ElapsedMilliseconds });

                //before generateKeyPair { { ElapsedMilliseconds = 2 } }
                //after generateKeyPair { { ElapsedMilliseconds = 1130 } }

            }
            catch
            {
                throw;
            }
        }

        // jsc shall auto rethrow java methods that throw!
        // async rewriter causes issues for javac otherwise?

        //public async Task<EncryptedBytes> Encrypt(byte[] data)
        public Task<EncryptedBytes> Encrypt(byte[] data)
        {
            Console.WriteLine("enter Encrypt");

            var value = default(EncryptedBytes);
            try
            {
                var rsaCipher = Cipher.getInstance("RSA");


                //Encrypt
                rsaCipher.init(Cipher.ENCRYPT_MODE, keyPair.getPublic());
                var encByte = (byte[])(object)rsaCipher.doFinal((sbyte[])(object)data);

                value = new EncryptedBytes(encByte);
            }
            catch
            {
                throw;
            }

            return value.AsResult();
        }

        //public async Task<byte[]> Decrypt(EncryptedBytes data)
        public Task<byte[]> Decrypt(EncryptedBytes data)
        {
            Console.WriteLine("enter Decrypt");

            var value = default(byte[]);
            try
            {

                var rsaCipher = Cipher.getInstance("RSA");


                //Decrypt
                rsaCipher.init(Cipher.DECRYPT_MODE, keyPair.getPrivate());
                value = (byte[])(object)rsaCipher.doFinal((sbyte[])(object)data.bytes);
            }
            catch
            {
                throw;
            }


            return value.AsResult();
        }
    }
}