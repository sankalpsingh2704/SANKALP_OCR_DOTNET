using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace InvoiceNew
{
    /// <summary>
    /// Summary description for Crypto.
    /// Encryption and Decryption Methods used for the Password.
    /// </summary>
    public class Crypto
    {
        private Byte[] KEY_64 = { 1, 2, 3, 4, 5, 6, 7, 8 };
        private Byte[] IV_64 = { 8, 7, 6, 5, 4, 3, 2, 1 };
        public Crypto()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public Crypto(string password)
        {
            byte[] pwBytes = Encoding.UTF8.GetBytes(password);
            for (int idx = 0; ((idx < pwBytes.Length) && (idx < 7)); idx++)
            {
                KEY_64[idx] = pwBytes[idx];
            }
        }

        // returns DES encrypted string 
        public string Encrypt(string value)
        {
            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, cryptoProvider.CreateEncryptor(KEY_64, IV_64), CryptoStreamMode.Write);
            StreamWriter sw = new StreamWriter(cs);
            sw.Write(value); sw.Flush();
            cs.FlushFinalBlock();
            ms.Flush();
            // convert back to a string  
            return Convert.ToBase64String(ms.GetBuffer(), 0, Convert.ToInt32(ms.Length));
        }

        // returns DES decrypted string 
        public string Decrypt(string value)
        {
            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            Byte[] buffer = Convert.FromBase64String(value);
            MemoryStream ms = new MemoryStream(buffer);
            CryptoStream cs = new CryptoStream(ms, cryptoProvider.CreateDecryptor(KEY_64, IV_64), CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }
    }
}
