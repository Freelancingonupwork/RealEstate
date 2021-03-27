using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Utills
{
    public class Encryption
    {
        private const string passPhrase = "Pas5pr@Pci#"; //"Pas5pr@se";

        public Encryption()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public static string EncryptionKey { get { return "#?&%@,:*"; } }
        public static string EncryptText(string sText)
        {
            return Encrypt(sText, EncryptionKey);

        }
        public static string Encrypt(string sText, string strEncrKey)
        {


            byte[] byKey = { };
            byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xab, 0xcd, 0xef };
            try
            {
                //byKey = System.Text.Encoding.UTF8.GetBytes(Left(strEncrKey, 8));
                byKey = System.Text.Encoding.UTF8.GetBytes(strEncrKey);
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = Encoding.UTF8.GetBytes(sText);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(byKey, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        public static string DecryptText(string sText)
        {
            return Decrypt(sText.ToString().Replace(" ", "+"), EncryptionKey);
        }


        private static string Decrypt(string sText, string sDecrKey)
        {
            byte[] byKey = { };
            byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xab, 0xcd, 0xef };
            byte[] inputByteArray = new byte[sText.Length + 1];
            try
            {

                byKey = System.Text.Encoding.UTF8.GetBytes(sDecrKey);
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByteArray = Convert.FromBase64String(sText);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                System.Text.Encoding encoding = System.Text.Encoding.UTF8;
                return encoding.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
