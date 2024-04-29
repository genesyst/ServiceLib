using System;
using System.Collections.Generic;
using System.Data;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
//using Excel = Microsoft.Office.Interop.Excel;

namespace ServiceLib
{
    public class ServiceIO
    {
        public static bool Copy(string inputFilePath, string outputFilePath)
        {
            bool Res = false;
            try
            {
                int bufferSize = 1024 * 1024;

                using (FileStream fileStream = new FileStream(outputFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                //using (FileStream fs = File.Open(<file-path>, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    FileStream fs = new FileStream(inputFilePath, FileMode.Open, FileAccess.ReadWrite);
                    fileStream.SetLength(fs.Length);
                    int bytesRead = -1;
                    byte[] bytes = new byte[bufferSize];

                    while ((bytesRead = fs.Read(bytes, 0, bufferSize)) > 0)
                    {
                        fileStream.Write(bytes, 0, bytesRead);
                    }

                    Res = true;
                }
            }
            catch { }
            return Res;
        }

        public static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        public static bool ByteArrayToFile(string fileName, byte[] byteArray)
        {
            try
            {
                using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(byteArray, 0, byteArray.Length);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in process: {0}", ex);
                return false;
            }
        }

        //public static void EntityToExcelSheet(string excelFilePath,string sheetName, IQueryable result, ObjectContext ctx)
        //{
        //    Excel.Application oXL;
        //    Excel.Workbook oWB;
        //    Excel.Worksheet oSheet;
        //    Excel.Range oRange;
        //    try
        //    {
        //        // Start Excel and get Application object.
        //        oXL = new Excel.Application();

        //        // Set some properties
        //        oXL.Visible = true;
        //        oXL.DisplayAlerts = false;

        //        // Get a new workbook. 
        //        oWB = oXL.Workbooks.Add(Missing.Value);

        //        // Get the active sheet 
        //        oSheet = (Excel.Worksheet)oWB.ActiveSheet;
        //        oSheet.Name = sheetName;

        //        // Process the DataTable
        //        // BE SURE TO CHANGE THIS LINE TO USE *YOUR* DATATABLE 
        //        DataTable dt = new ServiceData().EntityToDataTable(result, ctx); 

        //        int rowCount = 1;
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            rowCount += 1;
        //            for (int i = 1; i < dt.Columns.Count + 1; i++)
        //            {
        //                // Add the header the first time through 
        //                if (rowCount == 2)
        //                    oSheet.Cells[1, i] = dt.Columns[i - 1].ColumnName;
        //                oSheet.Cells[rowCount, i] = dr[i - 1].ToString();
        //            }
        //        }

        //        // Resize the columns 
        //        oRange = oSheet.Range[oSheet.Cells[1, 1], oSheet.Cells[rowCount, dt.Columns.Count]];
        //        oRange.Columns.AutoFit();

        //        // Save the sheet and close 
        //        oSheet = null;
        //        oRange = null;
        //        oWB.SaveAs(excelFilePath, Excel.XlFileFormat.xlWorkbookNormal, Missing.Value,
        //          Missing.Value, Missing.Value, Missing.Value,
        //          Excel.XlSaveAsAccessMode.xlExclusive, Missing.Value,
        //          Missing.Value, Missing.Value, Missing.Value);
        //        oWB.Close(Missing.Value, Missing.Value, Missing.Value);
        //        oWB = null;
        //        oXL.Quit();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public static long GetDirectorySize(string path)
        {
            // 1.
            // Get array of all file names.
            string[] a = Directory.GetFiles(path, "*.*");

            // 2.
            // Calculate total bytes of all files in a loop.
            long b = 0;
            foreach (string name in a)
            {
                // 3.
                // Use FileInfo to get length of each file.
                FileInfo info = new FileInfo(name);
                b += info.Length;
            }
            // 4.
            // Return total size
            return b;
        }

        public static byte[] EncryptBytes(byte[] inputBytes, string passPhrase, string saltValue)
        {
            RijndaelManaged RijndaelCipher = new RijndaelManaged();

            RijndaelCipher.Mode = CipherMode.CBC;
            byte[] salt = Encoding.ASCII.GetBytes(saltValue);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, salt, "SHA1", 2);

            ICryptoTransform Encryptor = RijndaelCipher.CreateEncryptor(password.GetBytes(32), password.GetBytes(16));

            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, Encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(inputBytes, 0, inputBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] CipherBytes = memoryStream.ToArray();

            memoryStream.Close();
            cryptoStream.Close();

            return CipherBytes;
        }

        public static byte[] DecryptBytes(byte[] encryptedBytes, string passPhrase, string saltValue)
        {
            RijndaelManaged RijndaelCipher = new RijndaelManaged();

            RijndaelCipher.Mode = CipherMode.CBC;
            byte[] salt = Encoding.ASCII.GetBytes(saltValue);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, salt, "SHA1", 2);

            ICryptoTransform Decryptor = RijndaelCipher.CreateDecryptor(password.GetBytes(32), password.GetBytes(16));

            MemoryStream memoryStream = new MemoryStream(encryptedBytes);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, Decryptor, CryptoStreamMode.Read);
            byte[] plainBytes = new byte[encryptedBytes.Length];

            int DecryptedCount = cryptoStream.Read(plainBytes, 0, plainBytes.Length);

            memoryStream.Close();
            cryptoStream.Close();

            return plainBytes;
        }
    }
}
