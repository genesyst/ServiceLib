using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace ServiceLib
{
    public class ServiceText
    {
        public static DateTime? APIParamToDate(string DateParam)
        {
            DateTime? Res = null;
            try
            {
                string[] paramDate = DateParam.Split('-');
                DateTime date = new DateTime(Convert.ToInt32(paramDate[2]), 
                                                Convert.ToInt32(paramDate[1]), 
                                                Convert.ToInt32(paramDate[0])
                                                );
                Res = date;
            }
            catch { throw; }
            return Res;
        }

        public static DateTime? APIParamToDateTime(string DateParam)
        {
            DateTime? Res = null;
            try
            {
                string[] paramDate = DateParam.Split('-');
                DateTime date = new DateTime(Convert.ToInt32(paramDate[2]),
                                                Convert.ToInt32(paramDate[1]),
                                                Convert.ToInt32(paramDate[0]),
                                                Convert.ToInt32(paramDate[3]),
                                                Convert.ToInt32(paramDate[4]),
                                                Convert.ToInt32(paramDate[5])
                                                );
                Res = date;
            }
            catch { throw; }
            return Res;
        }

        public static string StringToMoney(string moneyString)
        {
            decimal value = 0;
            if(decimal.TryParse(moneyString,out value))
            {
                return value.ToString("#,##0.00");
            }
            else
            {
                return moneyString;
            }
        }

        public static string StringToMoney(string moneyString,string format,string empty)
        {
            if (string.IsNullOrEmpty(moneyString))
            {
                return empty;
            }
            else
            {
                decimal value = 0;
                if (decimal.TryParse(moneyString, out value))
                {
                    return value.ToString(format);
                }
                else
                {
                    return moneyString;
                }
            }
        }

        public static string CreateRandomPassword(int length = 15)
        {
            // Create a string of characters, numbers, special characters that allowed in the password  
            string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";
            Random random = new Random();

            // Select one random character at a time from the string  
            // and create an array of chars  
            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = validChars[random.Next(0, validChars.Length)];
            }
            return new string(chars);
        }

        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static string ThaiBaht2Text(string txt)
        {
            string bahtTxt, n, bahtTH = "";
            double amount;
            try { amount = Convert.ToDouble(txt); }
            catch { amount = 0; }
            bahtTxt = amount.ToString("####.00");
            string[] num = { "ศูนย์", "หนึ่ง", "สอง", "สาม", "สี่", "ห้า", "หก", "เจ็ด", "แปด", "เก้า", "สิบ" };
            string[] rank = { "", "สิบ", "ร้อย", "พัน", "หมื่น", "แสน", "ล้าน" };
            string[] temp = bahtTxt.Split('.');
            string intVal = temp[0];
            string decVal = temp[1];
            if (Convert.ToDouble(bahtTxt) == 0)
                bahtTH = "ศูนย์บาทถ้วน";
            else
            {
                for (int i = 0; i < intVal.Length; i++)
                {
                    n = intVal.Substring(i, 1);
                    if (n != "0")
                    {
                        if ((i == (intVal.Length - 1)) && (n == "1"))
                            bahtTH += "เอ็ด";
                        else if ((i == (intVal.Length - 2)) && (n == "2"))
                            bahtTH += "ยี่";
                        else if ((i == (intVal.Length - 2)) && (n == "1"))
                            bahtTH += "";
                        else
                            bahtTH += num[Convert.ToInt32(n)];
                        bahtTH += rank[(intVal.Length - i) - 1];
                    }
                }
                bahtTH += "บาท";
                if (decVal == "00")
                    bahtTH += "ถ้วน";
                else
                {
                    for (int i = 0; i < decVal.Length; i++)
                    {
                        n = decVal.Substring(i, 1);
                        if (n != "0")
                        {
                            if ((i == decVal.Length - 1) && (n == "1"))
                                bahtTH += "เอ็ด";
                            else if ((i == (decVal.Length - 2)) && (n == "2"))
                                bahtTH += "ยี่";
                            else if ((i == (decVal.Length - 2)) && (n == "1"))
                                bahtTH += "";
                            else
                                bahtTH += num[Convert.ToInt32(n)];
                            bahtTH += rank[(decVal.Length - i) - 1];
                        }
                    }
                    bahtTH += "สตางค์";
                }
            }
            return bahtTH;
        }

    }
}
