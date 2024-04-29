using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;

namespace ServiceLib
{
    public class ServiceQR
    {
        private string QRFile;
        public ServiceQR(string qrFilePath)
        {
            this.QRFile = qrFilePath;
        }

        public void GenerateQCCode(string QCText)
        {
            try
            {
                var QCwriter = new BarcodeWriter();
                QCwriter.Format = BarcodeFormat.QR_CODE;
                var result = QCwriter.Write(QCText);

                var barcodeBitmap = new Bitmap(result);

                using (MemoryStream memory = new MemoryStream())
                {
                    using (FileStream fs = new FileStream(this.QRFile, FileMode.Create, FileAccess.ReadWrite))
                    {
                        barcodeBitmap.Save(memory, ImageFormat.Jpeg);
                        byte[] bytes = memory.ToArray();
                        fs.Write(bytes, 0, bytes.Length);
                    }
                }
            }
            catch { throw; }
        }

        public string ReadQRCode(string QCfilename)
        {
            try
            {
                var QCreader = new BarcodeReader();
                var QCresult = QCreader.Decode(new Bitmap(QCfilename));
                if (QCresult != null)
                {
                    return QCresult.Text;
                }
            }
            catch { throw; }

            return null;
        }

        public string QR2Base64Str()
        {
            if (File.Exists(this.QRFile))
                return ServiceImage.ImageToBase64String(this.QRFile);
            else
                return null;
        }
    }
}
