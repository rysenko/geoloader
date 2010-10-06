using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using GeoLoader.Properties;

namespace GeoLoader.Business.Savers
{
    public class GpsInfoSaver
    {
        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        public static void WriteLongLat(string filename, double latitude, double longitude)
        {
            var image = Image.FromFile(filename);
            image.Save(filename + ".tmp", ImageFormat.Bmp);
            image.Dispose();
            image = Image.FromFile(filename + ".tmp");

            // GPS Tag Version
            PropertyItem pitem = CreateNewPropertyItem(0x0);
            pitem.Value[0] = 2;
            pitem.Value[1] = 2;
            image.SetPropertyItem(pitem);

            // Latitude
            pitem = CreateNewPropertyItem(0x2);
            pitem.Value = GetExifBytes(latitude);
            image.SetPropertyItem(pitem);

            // LatitudeRef (North  or South)
            pitem = CreateNewPropertyItem(0x1);
            pitem.Value[0] = (byte) (latitude < 0 ? 'S' : 'N');
            image.SetPropertyItem(pitem);

            // Longitude
            pitem = CreateNewPropertyItem(0x4);
            pitem.Value = GetExifBytes(longitude);
            image.SetPropertyItem(pitem);

            // LatitudeRef (East or West)
            pitem = CreateNewPropertyItem(0x3);
            pitem.Value[0] = (byte) (longitude < 0 ? 'W' : 'E');
            image.SetPropertyItem(pitem);

            image.Save(filename, ImageFormat.Jpeg);
            image.Dispose();
            File.Delete(filename + ".tmp");
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        public static byte[] GetExifBytes(double coordinate)
        {
            var absCoord = Math.Abs(coordinate);

            byte degrees = (byte) absCoord;
            double minutesFloat = (absCoord - degrees) * 60;
            byte minutes = (byte) minutesFloat;
            double seconds = Math.Round((minutesFloat - minutes) * 60, 2);
            var bytes = new byte[24];
            for (int i = 0; i < 24; i++)
                bytes[i] = 0;

            bytes[0] = degrees;
            bytes[4] = 1;
            bytes[8] = minutes;
            bytes[12] = 1;

            int secondsInt = (int)(seconds * 100);

            byte[] temp = BitConverter.GetBytes(secondsInt);
            Array.Copy(temp, 0, bytes, 16, 4);
            bytes[20] = 100;

            return bytes;
        }

        private static PropertyItem CreateNewPropertyItem(int id)
        {
            return Resources.GPS.GetPropertyItem(id);
        }
    }
}
