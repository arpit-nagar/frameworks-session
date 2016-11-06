using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Tavisca.Frameworks.Session.Exceptions;
using Tavisca.Frameworks.Session.Resources;

namespace Tavisca.Frameworks.Session.Infrastructure
{
    internal static class Extensions
    {
        public static object Construct(this string typeInfo)
        {
            if (string.IsNullOrWhiteSpace(typeInfo))
                throw new ArgumentNullException("typeInfo");

            var type = Type.GetType(typeInfo, false, false);

            if (type == null)
                throw new SessionConfigurationException(string.Format(SessionResources.Type_Not_Found, typeInfo));

            return Activator.CreateInstance(type);
        }

        private static string _key;
        public static string GetKey()
        {
            if (_key == null)
            {
                var section = ConfigurationManager.GetSection("SecureAppSettings") as NameValueCollection;

                if (section != null)
                    _key = section["EncryptionKey"];
                else
                    throw new SessionConfigurationException(SessionResources.EncryptionKeyRequired);
            }

            return _key;
        }

        public static string TripleDESEncrypt(this string toEncrypt)
        {
            if (toEncrypt == null)
                return null;

            var bytes1 = Encoding.UTF8.GetBytes(toEncrypt);
            var bytes2 = Encoding.UTF8.GetBytes(GetKey());

            byte[] inArray;
            using (var cryptoServiceProvider = new TripleDESCryptoServiceProvider())
            {
                cryptoServiceProvider.Key = bytes2;
                cryptoServiceProvider.Mode = CipherMode.ECB;
                cryptoServiceProvider.Padding = PaddingMode.PKCS7;
                inArray = cryptoServiceProvider.CreateEncryptor().TransformFinalBlock(bytes1, 0, bytes1.Length);
            }
            return Convert.ToBase64String(inArray, 0, inArray.Length);
        }

        public static string TripleDESDecrypt(this string toDecrypt)
        {
            var inputBuffer = Convert.FromBase64String(toDecrypt);
            var bytes1 = Encoding.UTF8.GetBytes(GetKey());
            byte[] bytes2;
            using (var cryptoServiceProvider = new TripleDESCryptoServiceProvider())
            {
                cryptoServiceProvider.Key = bytes1;
                cryptoServiceProvider.Mode = CipherMode.ECB;
                cryptoServiceProvider.Padding = PaddingMode.PKCS7;
                bytes2 = cryptoServiceProvider.CreateDecryptor().TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
            }
            return Encoding.UTF8.GetString(bytes2);
        }
    }
}
