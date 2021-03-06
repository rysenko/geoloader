﻿using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using GeoLoader.Properties;

namespace GeoLoader.Business
{
    public class Client
    {
        WebClient client = new WebClient();
        private string cachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                                "GeoLoader\\Cache");
        
        public string DownloadString(string url)
        {
            client.Encoding = System.Text.Encoding.UTF8;

            if (!Directory.Exists(cachePath))
            {
                Directory.CreateDirectory(cachePath);
            }
            var urlHash = CalculateHash(url);
            var cacheFilePath = Path.Combine(cachePath, urlHash);
            var redownloadFile = true;
            if (File.Exists(cacheFilePath))
            {
                var cacheWriteTime = File.GetLastWriteTime(cacheFilePath);
                var timeDiff = DateTime.Now.Subtract(cacheWriteTime);
                if (timeDiff.Days < Settings.Default.CacheExpirationInDays)
                {
                    redownloadFile = false;
                }
            }
            string result;
            if (redownloadFile)
            {
                Task.Delay(50).Wait();
                result = client.DownloadString(url);
                File.WriteAllText(cacheFilePath, result);
            }
            else
            {
                result = File.ReadAllText(cacheFilePath);
            }
            return result;
        }

        public byte[] DownloadData(string url)
        {
            if (!Directory.Exists(cachePath))
            {
                Directory.CreateDirectory(cachePath);
            }
            var urlHash = CalculateHash(url);
            var cacheFilePath = Path.Combine(cachePath, urlHash);
            var redownloadFile = true;
            if (File.Exists(cacheFilePath))
            {
                var cacheWriteTime = File.GetLastWriteTime(cacheFilePath);
                var timeDiff = DateTime.Now.Subtract(cacheWriteTime);
                if (timeDiff.Days < Settings.Default.CacheExpirationInDays)
                {
                    redownloadFile = false;
                }
            }
            byte[] result;
            if (redownloadFile)
            {
                result = client.DownloadData(url);
                File.WriteAllBytes(cacheFilePath, result);
            }
            else
            {
                result = File.ReadAllBytes(cacheFilePath);
            }
            return result;
        }

        private string CalculateHash(string input)
        {
            var md5Provider = new MD5CryptoServiceProvider();
            byte[] data = Encoding.ASCII.GetBytes(input);
            data = md5Provider.ComputeHash(data);
            string strHash = string.Empty;
            foreach (byte b in data)
            {
                strHash += b.ToString("X2");
            }
            return strHash;
        }
    }
}