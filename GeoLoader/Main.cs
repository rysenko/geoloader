using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using BinaryAnalysis.UnidecodeSharp;
using GeoLoader.Business;
using GeoLoader.Business.Loaders;
using GeoLoader.Business.Savers;
using GeoLoader.Entities;
using GeoLoader.Properties;
using Region=GeoLoader.Entities.Region;

namespace GeoLoader
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private List<Country> countries;
        private List<Region> regions;
        private List<int> caches;

        private void Main_Load(object sender, EventArgs e)
        {
            var loader = new CountryLoader();
            countries = loader.List();
            foreach (var country in countries)
            {
                ddlCountry.Items.Add(country.Name);
            }
        }

        private int FindCountryIdByName(IEnumerable<Country> list,  string name)
        {
            foreach (var entity in list)
            {
                if (entity.Name == name)
                {
                    return entity.Id;
                }
            }
            return -1;
        }

        private int FindRegionIdByName(IEnumerable<Region> list, string name)
        {
            foreach (var entity in list)
            {
                if (entity.Name == name)
                {
                    return entity.Id;
                }
            }
            return -1;
        }

        private void ddlCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            var countryId = FindCountryIdByName(countries, ddlCountry.SelectedItem.ToString());
            var loader = new RegionLoader();
            regions = loader.List(countryId);
            ddlRegion.Items.Clear();
            foreach (var region in regions)
            {
                ddlRegion.Items.Add(region.Name);
            }
        }

        private void ddlRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            var countryId = FindCountryIdByName(countries, ddlCountry.SelectedItem.ToString());
            var regionId = FindRegionIdByName(regions, ddlRegion.SelectedItem.ToString());
            var loader = new CacheListLoader();
            lblCaches.Text = "Listing Caches...";
            caches = loader.List(countryId, regionId);
            lblCaches.Text = "Caches: " + caches.Count;
            btnSave.Enabled = true;
        }

        private string GetRegionFileName(string fileName)
        {
            var spacePos = fileName.IndexOf(" ");
            if (spacePos != -1)
            {
                fileName = fileName.Substring(0, spacePos);
                fileName = fileName.TrimEnd(',');
            }
            return fileName.Unidecode() + ".gpx";
        }

        internal class SavingWorkerArgument
        {
            public string SelectedPath;
            public string SelectedRegion;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if(!savingWorker.IsBusy)
            {
                var dialog = new FolderBrowserDialog();
                dialog.ShowDialog();
                if (dialog.SelectedPath != "")
                {
                    btnSave.Text = "Cancel";
                    savingWorker.RunWorkerAsync(new SavingWorkerArgument { SelectedPath = dialog.SelectedPath, SelectedRegion = ddlRegion.SelectedItem.ToString() });
                }
            }
            else
            {
                savingWorker.CancelAsync();
                btnSave.Text = "Save";
            }
        }

        private void savingWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var argument = e.Argument as SavingWorkerArgument;
            var cachesLoaded = 0;
            savingWorker.ReportProgress(0, "Loading Caches...");
            var cachesList = new List<GeoCache>();
            foreach (var cacheId in caches)
            {
                if (savingWorker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
                var cacheEntity = new GeoCacheLoader(cacheId).Load();
                cachesList.Add(cacheEntity);
                savingWorker.ReportProgress(cachesLoaded * 100 / caches.Count);
                cachesLoaded++;
            }
            savingWorker.ReportProgress(100, "Saving Caches...");
            var gpxFolderPath = Path.Combine(argument.SelectedPath, "GPX");
            if (!Directory.Exists(gpxFolderPath)) Directory.CreateDirectory(gpxFolderPath);
            var gpxFilePath = Path.Combine(gpxFolderPath, GetRegionFileName(argument.SelectedRegion));
            var fs = File.Create(gpxFilePath);
            new GeoCacheListSaver(cachesList).Save(fs);
            fs.Flush();
            fs.Close();
            if (Settings.Default.GeotagAndSaveCachePhotos)
            {
                savingWorker.ReportProgress(0, "Saving Images...");
                var imagesFolderPath = Path.Combine(argument.SelectedPath, "JPEG");
                if (!Directory.Exists(imagesFolderPath)) Directory.CreateDirectory(imagesFolderPath);
                var imagesSaved = 0;
                foreach (var cache in cachesList)
                {
                    if (savingWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }
                    var client = new Client();
                    try
                    {
                        var imageData =
                            client.DownloadData("http://www.geocaching.su/photos/caches/" + cache.Id + ".jpg");
                        var imagePath = imagesFolderPath + "\\" + cache.Id + ".jpg";
                        File.WriteAllBytes(imagePath, imageData);
                        GpsInfoSaver.WriteLongLat(imagePath, cache.Latitude, cache.Longitude);
                        savingWorker.ReportProgress(imagesSaved * 100 / cachesList.Count);
                    }
                    catch
                    {
                        savingWorker.ReportProgress(imagesSaved * 100 / cachesList.Count, "Skipped Image " + cache.Id);
                    }
                    imagesSaved++;
                }
            }
        }

        private void savingWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                lblCaches.Text = "Canceled!";
            }
            else if (e.Error != null)
            {
                lblCaches.Text = "Error: " + e.Error.Message;
            }
            else
            {
                lblCaches.Text = "Done!";
                btnSave.Text = "Save";
            }
        }

        private void savingWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var userState = e.UserState as string;
            if (!string.IsNullOrEmpty(userState))
            {
                lblCaches.Text = userState;
            }
            prgSaving.Value = e.ProgressPercentage;
        }
    }
}
