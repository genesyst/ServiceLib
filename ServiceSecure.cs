using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLib
{
    public class ServiceSecure
    {
        public ServiceSecure()
        {

        }

        public static string GenerateTokenByTime(DateTime datetime)
        {
            byte[] time = BitConverter.GetBytes(datetime.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            return Convert.ToBase64String(time.Concat(key).ToArray());
        }

        public static bool ValidateTokenByTime(string token)
        {
            byte[] data = Convert.FromBase64String(token);
            DateTime when = DateTime.FromBinary(BitConverter.ToInt64(data, 0));
            if (when < DateTime.Now)
            {
                // too old
                return false;
            }

            return true;
        }

        public string CreateRandomPassword(int charLenRequest)
        {
            string Res = "";
            // Create a string of characters, numbers, special characters that allowed in the password  
            string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";// +"!@#$%^&*?_-";
            Random random = new Random();

            while(Res.Length < charLenRequest)
            {
                try
                {
                    int rd_value = random.Next(0, validChars.Length);
                    Res += validChars[rd_value];
                }
                catch {
                    Res = "";
                }
            }

            return Res;
        }

        public string CreateRandomPasswordWithSpecial(int charLenRequest)
        {
            string Res = "";
            // Create a string of characters, numbers, special characters that allowed in the password  
            string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-+=:;.";
            Random random = new Random();

            while (Res.Length < charLenRequest)
            {
                try
                {
                    int rd_value = random.Next(0, validChars.Length);
                    Res += validChars[rd_value];
                }
                catch
                {
                    Res = "";
                }
            }

            return Res;
        }

        public string OTPGenerate(int digit)
        {
            string Res = "";
            try
            {
                Random rd = new Random();
                while(Res.Length < digit)
                {
                    int minRd = DateTime.Now.Minute * DateTime.Now.Second;
                    int maxRd = unchecked((int)DateTime.Now.Ticks);
                    int dit = rd.Next(999999);
                    if (minRd < maxRd)
                        dit = rd.Next(minRd, maxRd);
                    else if(minRd > maxRd)
                        dit = rd.Next(maxRd, minRd);

                    string rdValue = dit.ToString();
                    int rdValueLenLast = rdValue.Length - 1;
                    Res += rdValue[rdValueLenLast];
                }
            }
            catch { throw; }
            return Res;
        }

        public string GenerateToken(string UserCode,string LongCode ,string SecretCode)
        {
            string Res = LongCode.Replace("-","").ToUpper();
            Res = this.EncryptData(LongCode, SecretCode);
            Res = Res.Replace("=", "");

            if(Res.Length >= 32)
                Res = Res.Substring(Res.Length - 32);

            return UserCode + "@" + Res;
        }

        public string GenerateSecretCode(string Value)
        {
            string Res = null;

            int newInt = Value.Trim().GetHashCode() + DateTime.Now.GetHashCode();
            Res = newInt.ToString().Replace("-","");
            if (Res.Length > 20)
                Res = Res.Substring(0, 20);

            return Res;
        }

        public string GetMd5Hash(MD5 md5Hash, string input)
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input.Trim()));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        public bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
        {
            // Hash the input.
            string hashOfInput = GetMd5Hash(md5Hash, input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetTokenEncrypt(string Value)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                return new ServiceSecure().GetMd5Hash(md5Hash, Value);
            }
        }

        public string EncryptData(string textData, string Encryptionkey)
        {
            RijndaelManaged objrij = new RijndaelManaged();
            //set the mode for operation of the algorithm   
            objrij.Mode = CipherMode.CBC;
            //set the padding mode used in the algorithm.   
            objrij.Padding = PaddingMode.PKCS7;
            //set the size, in bits, for the secret key.   
            objrij.KeySize = 0x80;
            //set the block size in bits for the cryptographic operation.    
            objrij.BlockSize = 0x80;
            //set the symmetric key that is used for encryption & decryption.    
            byte[] passBytes = Encoding.UTF8.GetBytes(Encryptionkey);
            //set the initialization vector (IV) for the symmetric algorithm    
            byte[] EncryptionkeyBytes = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            int len = passBytes.Length;
            if (len > EncryptionkeyBytes.Length)
            {
                len = EncryptionkeyBytes.Length;
            }
            Array.Copy(passBytes, EncryptionkeyBytes, len);

            objrij.Key = EncryptionkeyBytes;
            objrij.IV = EncryptionkeyBytes;

            //Creates symmetric AES object with the current key and initialization vector IV.    
            ICryptoTransform objtransform = objrij.CreateEncryptor();
            byte[] textDataByte = Encoding.UTF8.GetBytes(textData);
            //Final transform the test string.  
            return Convert.ToBase64String(objtransform.TransformFinalBlock(textDataByte, 0, textDataByte.Length));
        }

        public string DecryptData(string EncryptedText, string Encryptionkey)
        {
            RijndaelManaged objrij = new RijndaelManaged();
            objrij.Mode = CipherMode.CBC;
            objrij.Padding = PaddingMode.PKCS7;

            objrij.KeySize = 0x80;
            objrij.BlockSize = 0x80;
            byte[] encryptedTextByte = Convert.FromBase64String(EncryptedText);
            byte[] passBytes = Encoding.UTF8.GetBytes(Encryptionkey);
            byte[] EncryptionkeyBytes = new byte[0x10];
            int len = passBytes.Length;
            if (len > EncryptionkeyBytes.Length)
            {
                len = EncryptionkeyBytes.Length;
            }
            Array.Copy(passBytes, EncryptionkeyBytes, len);
            objrij.Key = EncryptionkeyBytes;
            objrij.IV = EncryptionkeyBytes;
            byte[] TextByte = objrij.CreateDecryptor().TransformFinalBlock(encryptedTextByte, 0, encryptedTextByte.Length);
            return Encoding.UTF8.GetString(TextByte);  //it will return readable string  
        }

        public static string encryptString(string value)
        {
            string res = "";
            try
            {

                MemoryStream encStream = new MemoryStream();
                RC2CryptoServiceProvider RC2 = new RC2CryptoServiceProvider();
                byte[] iv = { 15, 0, 53, 255, 78, 25, 72, 84 };
                RC2.IV = iv;
                RC2.Key = Encoding.Default.GetBytes("123456");
                byte[] Cypher = Encoding.Default.GetBytes(value);
                CryptoStream cryStm = new CryptoStream(encStream, RC2.CreateEncryptor(), CryptoStreamMode.Write);
                cryStm.Write(Cypher, 0, Cypher.Length);
                cryStm.Close();
                res = Convert.ToBase64String(encStream.ToArray());


            }
            catch
            {

            }
            return res;
        }

        public static string decyptString(string value)
        {
            string res = "";
            try
            {
                RC2CryptoServiceProvider RC2 = new RC2CryptoServiceProvider();
                byte[] iv = { 15, 0, 53, 255, 78, 25, 72, 84 };
                RC2.IV = iv;
                RC2.Key = Encoding.Default.GetBytes("123456");
                byte[] Cypher = Convert.FromBase64String(value);
                MemoryStream decStream = new MemoryStream(Cypher);
                CryptoStream cryStm = new CryptoStream(decStream, RC2.CreateDecryptor(), CryptoStreamMode.Read);
                StreamReader rd = new StreamReader(cryStm, Encoding.Default);
                res = rd.ReadLine();
                cryStm.Close();
            }
            catch
            {

            }
            return res;
        }
    }
}
