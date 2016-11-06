using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tavisca.Frameworks.Session.KeyGen
{
    public partial class KeyGen : Form
    {
        public KeyGen()
        {
            InitializeComponent();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            var input = txtInput.Text.Replace("url|||tableName|||AccessKey|||SecretKey", string.Empty);

            if (string.IsNullOrWhiteSpace(input))
            {
                MessageBox.Show("Enter input", "Error");
                return;
            }

            var key = txtKey.Text;

            if (string.IsNullOrWhiteSpace(key))
            {
                MessageBox.Show("Enter key", "Error");
                return;
            }

            try
            {
                if (IsEncrypted(input, key))
                    txtOutput.Text = TripleDESDecrypt(input, key);
                else
                    txtOutput.Text = TripleDESEncrypt(input, key);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error!");
            }
        }

        public static string TripleDESEncrypt(string toEncrypt, string key)
        {
            if (toEncrypt == null)
                return null;

            var bytes1 = Encoding.UTF8.GetBytes(toEncrypt);
            var bytes2 = Encoding.UTF8.GetBytes(key);

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

        public static string TripleDESDecrypt(string toDecrypt, string key)
        {
            var inputBuffer = Convert.FromBase64String(toDecrypt);
            var bytes1 = Encoding.UTF8.GetBytes(key);
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

        public bool IsEncrypted(string text, string key)
        {
            if (!IsBase64(text))
                return false;

            try
            {
                TripleDESDecrypt(text, key);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsBase64(string value)
        {
            var trimmed = value.Trim();

            return (trimmed.Length % 4 == 0) && Regex.IsMatch(trimmed, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }
    }
}
