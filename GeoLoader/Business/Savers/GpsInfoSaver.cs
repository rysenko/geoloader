using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using GeoLoader.Properties;

namespace GeoLoader.Business.Savers
{
    public class GpsInfoSaver
    {
        public static byte[] WriteLongLat(byte[] fileData, double latitude, double longitude)
        {
            var inputMs = new MemoryStream(fileData);
            var image = Image.FromStream(inputMs);
            inputMs.Flush();
            var bitmapMs = new MemoryStream();
            image.Save(bitmapMs, ImageFormat.Tiff);
            image.Dispose();
            inputMs.Dispose();
            bitmapMs.Flush();
            image = Image.FromStream(bitmapMs);

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

            var outputMs = new MemoryStream();
            image.Save(outputMs, ImageFormat.Jpeg);
            image.Dispose();
            bitmapMs.Dispose();
            outputMs.Flush();
            return outputMs.GetBuffer();
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
