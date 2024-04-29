using Genesyst.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLib
{
    public enum DateCulture {ctThai,ctEng }
    public class ServiceDateTime
    {

        public static string[] MonthName = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October","November","Decenber" };
        public static string[] S_MonthName = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        public static string[] THMonthName = { "มกราคม", "กุมภาพันธ์", "มีนาคม", "เมษายน", "พฤษภาคม", "มิถุนายน", "กรกฎาคม", "สิงหาคม", "กันยายน", "ตุลาคม", "พฤศจิกายน", "ธันวาคม" };
        public static string[] S_THMonthName = { "ม.ค.", "ก.พ.", "มี.ค.", "เม.ย.", "พ.ค.", "มิ.ย.", "ก.ค.", "ส.ค.", "ก.ย.", "ต.ค.", "พ.ย.", "ธ.ค." };

        public static string[] WeekDayName = new string[] { "SUNDAY", "MONDAY", "TUESDAY", "WEDNESDAY", "THURSDAY", "FRIDAY", "SATURDAY" };
        public static string[] WeekDayNameTh = new string[] { "อาทิตย์", "จันทร์", "อังคาร", "พุธ", "พฤหัสบดี", "ศุกร์", "เสาร์" };


        public static string GetMonthString(string[] MonthArray)
        {
            string Res = "";
            foreach (string m in MonthArray)
                Res += "," + m;

            return Res.Substring(1);
        }

        public static List<mdlGValue> GetThaiMonths()
        {
            int i = 1;
            List<mdlGValue> Res = new List<mdlGValue>();
            foreach (string m in ServiceDateTime.THMonthName)
            {
                Res.Add(new mdlGValue() {
                    Key = i.ToString(),
                    Value = m
                });

                i++;
            }

            return Res;
        }

        public static string ToDateTimeString(DateTime setDate,string pattern, DateCulture culture)
        {
            if (culture == DateCulture.ctEng)
            {
                DateTimeFormatInfo usDtfi = new CultureInfo("en-US", false).DateTimeFormat;
                return setDate.ToString(pattern, usDtfi);
            }
            else
            {
                DateTimeFormatInfo usDtfi = new CultureInfo("th-TH", false).DateTimeFormat;
                return setDate.ToString(pattern, usDtfi);
            }
        }

        public static int GetMonthDifference(DateTime startDate, DateTime endDate)
        {
            int monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
            return Math.Abs(monthsApart);
        }
    }
}
