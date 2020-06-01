using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace MetReferenceDataService.Controllers
{
    public class Obfuscator
    {

        static bool init = false;
        static byte[] Key = new byte[24];


        static string ToHex(byte[] value)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in value)
                sb.AppendFormat("{0:x2}", b);
            return sb.ToString();
        }

        static public string Encode(string value, byte[] key)
        {
            try
            {

                TripleDESCryptoServiceProvider TDes = new TripleDESCryptoServiceProvider();
                TDes.Mode = CipherMode.ECB;
                TDes.Padding = PaddingMode.PKCS7;
                TDes.Key = key;

                Byte[] byteArray = Encoding.UTF8.GetBytes(value);


                MemoryStream memoryStream = new MemoryStream();

                CryptoStream cryptoStream = new CryptoStream(memoryStream,
                    TDes.CreateEncryptor(), CryptoStreamMode.Write);


                cryptoStream.Write(byteArray, 0, byteArray.Length);

                cryptoStream.FlushFinalBlock();

                return Convert.ToBase64String(memoryStream.ToArray());
            }
            catch (Exception ex)
            {
                string err = "<empty>";
                return err;
            }
        }

        static public bool TryDecode(string value, byte[] key, out string result)
        {

            byte[] InputBuffer = Convert.FromBase64String(value);


            TripleDESCryptoServiceProvider TDes = new TripleDESCryptoServiceProvider();
            TDes.Mode = CipherMode.ECB;
            TDes.Padding = PaddingMode.PKCS7;
            TDes.Key = key;

            MemoryStream memoryStream = new MemoryStream();

            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                TDes.CreateDecryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(InputBuffer, 0, InputBuffer.Length);

            cryptoStream.FlushFinalBlock();

            result = Encoding.UTF8.GetString(memoryStream.ToArray());

            return true;

        }

        static void Init()
        {
            (new RNGCryptoServiceProvider()).GetBytes(Key);
            init = true;
        }

        static public byte[] getKey()
        {
            if (!init)
                Init();
            return Key;
        }


        public Obfuscator()
        {
            Init();
        }


        static public void Test()
        {
            long NumberToEncode = (new Random()).Next();
            Console.WriteLine("Number to encode = {0}.", NumberToEncode);
            byte[] Key = new byte[24];
            string sNumberToEncode = NumberToEncode.ToString();
            (new RNGCryptoServiceProvider()).GetBytes(Key);
            Console.WriteLine("Key to encode with is {0}.", ToHex(Key));
            string EncodedValue = Encode(sNumberToEncode, Key);
            Console.WriteLine("The encoded value is {0}.", EncodedValue);
            long DecodedValue;
            string sDecodedValue;
            bool Success = TryDecode(EncodedValue, Key, out sDecodedValue);
            if (Success)
            {
                Console.WriteLine("Successfully decoded the encoded value.");
                Console.WriteLine("The decoded result is {0}.", sDecodedValue);
            }
            else
                Console.WriteLine("Failed to decode encoded value. Invalid result.");
        }

    }
}
