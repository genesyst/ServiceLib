using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLib
{
    public enum onImageLocation {imlButtomCenter, imlButtomLeft , imlButtomRight, imlCenter }
    public class ServiceImage
    {
        public ServiceImage()
        {

        }

        public static bool IsImage(string contentType)
        {
            return contentType.StartsWith("image/");
        }


        /// <summary>  
        /// resize an image and maintain aspect ratio  
        /// </summary>  
        /// <param name="image">image to resize</param>  
        /// <param name="newWidth">desired width</param>  
        /// <param name="maxHeight">max height</param>  
        /// <param name="onlyResizeIfWider">if image width is smaller than newWidth use image width</param>  
        /// <returns>resized image</returns>  
        public static Image Resize(Image image, int newWidth, int maxHeight, bool onlyResizeIfWider)
        {
            if (onlyResizeIfWider && image.Width <= newWidth) newWidth = image.Width;

            var newHeight = image.Height * newWidth / image.Width;
            if (newHeight > maxHeight)
            {
                // Resize with height instead  
                newWidth = image.Width * maxHeight / image.Height;
                newHeight = maxHeight;
            }

            var res = new Bitmap(newWidth, newHeight);

            using (var graphic = Graphics.FromImage(res))
            {
                graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphic.SmoothingMode = SmoothingMode.HighQuality;
                graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphic.CompositingQuality = CompositingQuality.HighQuality;
                graphic.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return res;
        }

        /// <summary>  
        /// Crop an image   
        /// </summary>  
        /// <param name="img">image to crop</param>  
        /// <param name="cropArea">rectangle to crop</param>  
        /// <returns>resulting image</returns>  
        public static Image Crop(Image img, Rectangle cropArea)
        {
            var bmpImage = new Bitmap(img);
            var bmpCrop = bmpImage.Clone(cropArea, bmpImage.PixelFormat);
            return bmpCrop;
        }

        public static byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }

        public static Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

        public static Image ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);
            Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);
            return newImage;
        }

        public static Image GetImageFromUrl(string url)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            // if you have proxy server, you may need to set proxy details like below 
            //httpWebRequest.Proxy = new WebProxy("proxyserver",port){ Credentials = new NetworkCredential(){ UserName ="uname", Password = "pw"}};

            try
            {
                using (HttpWebResponse httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (Stream stream = httpWebReponse.GetResponseStream())
                    {
                        return Image.FromStream(stream);
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public static bool CompareImages(Bitmap image1, Bitmap image2)
        {
            if (image1.Width == image2.Width && image1.Height == image2.Height)
            {
                for (int i = 0; i < image1.Width; i++)
                {
                    for (int j = 0; j < image1.Height; j++)
                    {
                        if (image1.GetPixel(i, j) != image2.GetPixel(i, j))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public static byte[] Resize2MaxKbytes(byte[] byteImageIn,int fixKB)
        {
            byte[] currentByteImageArray = byteImageIn;
            double scale = 1f;

            MemoryStream inputMemoryStream = new MemoryStream(byteImageIn);
            Image fullsizeImage = Image.FromStream(inputMemoryStream);

            while (currentByteImageArray.Length > (1000 * fixKB))
            {
                Bitmap fullSizeBitmap = new Bitmap(fullsizeImage, new Size((int)(fullsizeImage.Width * scale), (int)(fullsizeImage.Height * scale)));
                MemoryStream resultStream = new MemoryStream();

                fullSizeBitmap.Save(resultStream, fullsizeImage.RawFormat);

                currentByteImageArray = resultStream.ToArray();
                resultStream.Dispose();
                resultStream.Close();

                scale -= 0.05f;
            }

            return currentByteImageArray;
        }

        public static string ImageToBase64String(string imagePath)
        {
            string Res = null;
            try
            {
                using (Image image = Image.FromFile(imagePath))
                {
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes = m.ToArray();

                        // Convert byte[] to Base64 String
                        Res = Convert.ToBase64String(imageBytes);
                    }
                }
            }
            catch(Exception ex)
            {

            }
            return Res;
        }

        public static byte[] WriteTextToImage(byte[] imageFile,string Text,Brush brushColor, ImageFormat format )
        {
            byte[] Res = null;
            try
            {
                Bitmap bitmap;
                using (var ms = new MemoryStream(imageFile))
                {
                    bitmap = new Bitmap(ms);
                    int zonDev = 0;
                    int TextSize = new ServiceImage().getFontSize(bitmap.Width, bitmap.Height, ref zonDev);
                    RectangleF rectf = new RectangleF(20, 20, bitmap.Width - 20, zonDev);
                    RectangleF rectf_shadow = new RectangleF(22, 22, bitmap.Width - 20, zonDev);

                    Graphics g = Graphics.FromImage(bitmap);

                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    Font font = new Font("Tahoma", TextSize, FontStyle.Bold);
                    g.DrawString(Text, font, Brushes.Black, rectf_shadow);
                    g.DrawString(Text, font, brushColor, rectf);

                    g.Flush();

                    using (var memoryStream = new MemoryStream())
                    {
                        bitmap.Save(memoryStream, format);
                        Res = memoryStream.ToArray();
                    }
                }
            }
            catch(Exception ex)
            {
                throw;
            }

            return Res;
        }

        public static Bitmap WriteTextToBitmap(byte[] imageFile, string Text, Brush brushColor, ImageFormat format)
        {
            Bitmap Res = null;
            try
            {
                Bitmap bitmap;
                using (var ms = new MemoryStream(imageFile))
                {
                    bitmap = new Bitmap(ms);
                    int zonDev = 0;
                    int TextSize = new ServiceImage().getFontSize(bitmap.Width, bitmap.Height, ref zonDev);
                    RectangleF rectf = new RectangleF(20, 20, bitmap.Width - 20, zonDev);
                    RectangleF rectf_shadow = new RectangleF(22, 22, bitmap.Width - 20, zonDev);

                    Graphics g = Graphics.FromImage(bitmap);

                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    Font font = new Font("Tahoma", TextSize, FontStyle.Bold);
                    g.DrawString(Text, font, Brushes.Black, rectf_shadow);
                    g.DrawString(Text, font, brushColor, rectf);

                    g.Flush();

                    Res = bitmap;
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return Res;
        }

        public int getFontSize(int width, int height, ref int limitzone)
        {
            if (width < height) // portait
            {
                limitzone = height / 4;
                if (width >= 100 && width < 250)
                    return 10;
                else if (width >= 250 && width < 500)
                    return 12;
                else if (width >= 500 && width < 750)
                    return 20;
                else if (width >= 750 && width < 1000)
                    return 28;
                else if (width >= 1000 && width < 1500)
                    return 36;
                else if (width > 1500)
                    return 34;
            }
            else if (width > height)
            {
                limitzone = height / 4;
                if (width >= 100 && width < 250)
                    return 6;
                else if (width >= 250 && width < 500)
                    return 8;
                else if (width >= 500 && width < 750)
                    return 12;
                else if (width >= 750 && width < 1000)
                    return 16;
                else if (width >= 1000 && width < 1500)
                    return 20;
                else if (width > 1500)
                    return 24;
            }
            else
            {
                limitzone = height / 2;
                if (width >= 100 && width < 250)
                    return 6;
                else if (width >= 250 && width < 500)
                    return 8;
                else if (width >= 500 && width < 750)
                    return 12;
                else if (width >= 750 && width < 1000)
                    return 16;
                else if (width >= 1000 && width < 1500)
                    return 20;
                else if (width > 1500)
                    return 24;
            }

            return 10;
        }

        public static byte[] StampIconToImage(byte[] imageFile,string iconPath, onImageLocation stampLocation)
        {
            byte[] Res = null;
            try
            {
                using (Image image = ServiceImage.byteArrayToImage(imageFile))
                using (Image icon = Image.FromFile(iconPath))
                using (Graphics imageGraphics = Graphics.FromImage(image))
                using (TextureBrush watermarkBrush = new TextureBrush(icon))
                {
                    int x = 0;
                    int y = 0;
                    switch (stampLocation)
                    {
                        case onImageLocation.imlButtomCenter:
                            x = (image.Width / 2 - icon.Width / 2);
                            y = (image.Height - icon.Height) - 10;
                            break;
                        case onImageLocation.imlButtomLeft:
                            x = (image.Width - (image.Width - 10));
                            y = (image.Height - icon.Height) - 10;
                            break;
                        case onImageLocation.imlButtomRight:
                            x = (image.Width - (icon.Width + 10));
                            y = (image.Height - icon.Height) - 10;
                            break;
                        case onImageLocation.imlCenter:
                            x = (image.Width / 2 - icon.Width / 2);
                            y = (image.Height / 2 - icon.Height / 2);
                            break;
                        default:
                            x = (image.Width / 2 - icon.Width / 2);
                            y = (image.Height / 2 - icon.Height / 2);
                            break;
                    }

                    watermarkBrush.TranslateTransform(x, y);
                    imageGraphics.FillRectangle(watermarkBrush, new Rectangle(new Point(x, y), new Size(icon.Width + 1, icon.Height)));
                    Res = ServiceImage.imageToByteArray(image);
                }
            }
            catch(Exception ex)
            {
                throw;
            }

            return Res;
        }
    }
}
