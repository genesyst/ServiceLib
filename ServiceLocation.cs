using ServiceLib.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLib
{
    public class ServiceLocation
    {
        public ServiceLocation()
        {

        }

        public static RootObject getAddress(double lat, double lng)
        {
            try
            {
                WebClient webClient = new WebClient();
                webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                webClient.Headers.Add("Referer", "http://www.microsoft.com");
                var jsonData = webClient.DownloadData("http://nominatim.openstreetmap.org/reverse?format=json&lat=" + lat + "&lon=" + lng);
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(RootObject));
                RootObject rootObject = (RootObject)ser.ReadObject(new MemoryStream(jsonData));
                return rootObject;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public static RootObject getAddress(string location)
        {
            try
            {
                string[] gps = location.Split(',');
                WebClient webClient = new WebClient();
                webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                webClient.Headers.Add("Referer", "http://www.microsoft.com");
                var jsonData = webClient.DownloadData("http://nominatim.openstreetmap.org/reverse?format=json&lat=" + gps[0] + "&lon=" + gps[1]);
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(RootObject));
                RootObject rootObject = (RootObject)ser.ReadObject(new MemoryStream(jsonData));
                return rootObject;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string getAddressDisplay(string location,int limittext)
        {
            if (string.IsNullOrEmpty(location))
                return "ไม่มีตำแหน่งพิกัด";

            try
            {
                string[] gps = location.Split(',');
                WebClient webClient = new WebClient();
                webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                webClient.Headers.Add("Referer", "http://www.microsoft.com");
                var jsonData = webClient.DownloadData("http://nominatim.openstreetmap.org/reverse?format=json&lat=" + gps[0] + "&lon=" + gps[1]);
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(RootObject));
                RootObject rootObject = (RootObject)ser.ReadObject(new MemoryStream(jsonData));

                string res = ServiceLocation.AddressShortly(rootObject.display_name, limittext);

                return res;
            }
            catch(Exception ex)
            {
                return "";
            }
        }

        public static string AddressShortly(string address, int limittext)
        {
            //replace
            string res = address.Replace("ประเทศไทย", "");
            res = res.Replace("จังหวัด", "จ.");
            res = res.Replace("อำเภอ", "อ.");
            res = res.Replace("ตำบล", "ต.");
            res = res.Replace("ถนน", "ถ.");
            res = res.Replace("กรุงเทพมหานคร", "กทม.");
            res = res.Replace("โรงพยาบาล", "รพ.");
            //remove
            res = res.Replace("เทศบาล", "");
            res = res.Replace("เทศบาลนคร", "");
            res = res.Replace("เทศบาลเมือง", "");
            res = res.Replace("องค์การบริหารส่วน", "");
            res = res.Replace("ห้างสรรพสินค้า", "");
            res = res.Replace("โครงการ", "");
            res = res.Replace("ทางพิเศษ", "");
            res = res.Replace("ทางยกระดับ", "");

            if (res.Length > limittext)
                res = res.Substring(0, limittext - 1);

            return res;
        }

    }
}
