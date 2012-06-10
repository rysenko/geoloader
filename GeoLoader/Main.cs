using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using BinaryAnalysis.UnidecodeSharp;
using GeoLoader.Business;
using GeoLoader.Business.Loaders;
using GeoLoader.Business.Savers;
using GeoLoader.Entities;
using GeoLoader.Properties;
using Microsoft.Win32;

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
        private string wptName;
        private string poiLoaderPath;

        private void Main_Load(object sender, EventArgs e)
        {
            Text += " " + Assembly.GetExecutingAssembly().GetName().Version;
            var loader = new CountryLoader();
            countries = loader.List();
            countries.Add(new Country {Id = -1, Name = "Из .wpt файла..."});
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
            ddlRegion.Items.Clear();
            if (countryId == -1)
            {
                var dialog = new OpenFileDialog {Title = "Выберите wpt файл", Filter = "wpt files (*.wpt)|*.wpt", RestoreDirectory = true};
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var wptLister = new WptListLoader();
                    caches = wptLister.List(dialog.FileName);
                    wptName = Path.GetFileNameWithoutExtension(dialog.FileName);
                    btnSaveGpx.Enabled = true;
                    btnSavePoi.Enabled = true;
                    //btnSave_Click(sender, e);
                }
            }
            else
            {
                var loader = new RegionLoader();
                regions = loader.List(countryId);
                foreach (var region in regions)
                {
                    ddlRegion.Items.Add(region.Name);
                }
            }
            
        }

        private void ddlRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            var countryId = FindCountryIdByName(countries, ddlCountry.SelectedItem.ToString());
            var regionId = FindRegionIdByName(regions, ddlRegion.SelectedItem.ToString());
            var loader = new CacheListLoader();
            lblCaches.Text = "Загрузка списка...";
            caches = loader.List(countryId, regionId);
            lblCaches.Text = "Кэшей: " + caches.Count;
            btnSaveGpx.Enabled = true;
            btnSavePoi.Enabled = true;
        }

        private string GetRegionFileName(string fileName)
        {
            var spacePos = fileName.IndexOf(" ");
            if (spacePos != -1)
            {
                fileName = fileName.Substring(0, spacePos);
                fileName = fileName.TrimEnd(',');
            }
            return Unidecoder.Unidecode(fileName) + ".gpx";
        }

        internal class SavingWorkerArgument
        {
            public string SelectedPath;
            public string SelectedRegion;
            public bool PoiStyle;
        }

        private void btnSaveGpx_Click(object sender, EventArgs e)
        {
            btnSave_Click(false);
        }

        private void btnSavePoi_Click(object sender, EventArgs e)
        {
            poiLoaderPath = (string) Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Garmin\\Applications\\POI Loader", "InstallDir", "");
            if (string.IsNullOrEmpty(poiLoaderPath))
            {
                MessageBox.Show("Не найден Garmin POI Loader");
            } else {
                btnSave_Click(true);
            }
            
        }

        private void btnSave_Click(bool poiStyle)
        {
            if(!savingWorker.IsBusy)
            {
                var dialog = new FolderBrowserDialog();
                dialog.ShowDialog();
                if (dialog.SelectedPath != "")
                {
                    if (poiStyle)
                    {
                        btnSavePoi.Text = "X";
                        btnSaveGpx.Enabled = false;
                    }
                    else
                    {
                        btnSaveGpx.Text = "X";
                        btnSavePoi.Enabled = false;
                    }
                    
                    var selectedRegion = ddlRegion.SelectedItem != null ? ddlRegion.SelectedItem.ToString() : wptName;
                    savingWorker.RunWorkerAsync(new SavingWorkerArgument { SelectedPath = dialog.SelectedPath, SelectedRegion = selectedRegion, PoiStyle = poiStyle});
                }
            }
            else
            {
                savingWorker.CancelAsync();
                btnSaveGpx.Text = "GPX";
                btnSavePoi.Text = "POI";
                btnSaveGpx.Enabled = true;
                btnSavePoi.Enabled = true;
            }
        }

        private string GetRelateiveImagePath(string folderPath, string cacheFullId, int postIndex)
        {
            var lastChar = cacheFullId[cacheFullId.Length - 1];
            var secondToLast = cacheFullId[cacheFullId.Length - 2];
            var directory = folderPath + "\\" + lastChar + "\\" + secondToLast + "\\" + cacheFullId +
                (postIndex == 0 ? "\\Spoilers" : "");
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            return directory + "\\" + cacheFullId + (postIndex == 0 ? "" : "-" + postIndex) + ".jpg";
        }

        private void savingWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var argument = e.Argument as SavingWorkerArgument;
            var cachesLoaded = 0;
            savingWorker.ReportProgress(0, "Загрузка кэшей...");
            var cachesList = new List<GeoCache>();
            foreach (var cacheId in caches)
            {
                if (savingWorker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
                var cacheEntity = new GeoCacheLoader(cacheId).Load();
                if (string.IsNullOrEmpty(cacheEntity.Country))
                {
                    cacheEntity.Country = ddlCountry.Text;
                }
                if (string.IsNullOrEmpty(cacheEntity.State))
                {
                    cacheEntity.State = ddlRegion.Text;
                }
                cachesList.Add(cacheEntity);
                savingWorker.ReportProgress(cachesLoaded * 100 / caches.Count);
                cachesLoaded++;
            }
            savingWorker.ReportProgress(100, "Сохранение кэшей...");
            var gpxFolderPath = argument.PoiStyle ? argument.SelectedPath : Path.Combine(argument.SelectedPath, "GPX");
            if (!Directory.Exists(gpxFolderPath)) Directory.CreateDirectory(gpxFolderPath);
            var gpxFilePath = Path.Combine(gpxFolderPath, GetRegionFileName(argument.SelectedRegion));
            var fs = File.Create(gpxFilePath);
            new GeoCacheListSaver(cachesList, argument.PoiStyle).Save(fs);
            fs.Flush();
            fs.Close();
            if (Settings.Default.SaveCachePhotos && !Settings.Default.SaveMinimalInfo)
            {
                savingWorker.ReportProgress(0, "Сохранение картинок...");
                var imagesFolderPath = argument.PoiStyle ? argument.SelectedPath : Path.Combine(argument.SelectedPath, "GeocachePhotos");
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
                        if (!string.IsNullOrEmpty(cache.CacheImage))
                        {
                            var imageData = client.DownloadData(cache.CacheImage);
                            var imagePath = argument.PoiStyle ? imagesFolderPath + "\\" + cache.Id + ".jpg" :
                                GetRelateiveImagePath(imagesFolderPath, cache.FullId, 0);
                            File.WriteAllBytes(imagePath, imageData);
                        }
                        if (Settings.Default.SaveTerritoryPhotos && !argument.PoiStyle)
                        {
                            var territoryImageIndex = 1;
                            foreach (var territoryImage in cache.TerritoryImages)
                            {
                                var imageData = client.DownloadData(territoryImage);
                                var imagePath = GetRelateiveImagePath(imagesFolderPath, cache.FullId, territoryImageIndex);
                                File.WriteAllBytes(imagePath, imageData);
                                territoryImageIndex++;
                            }
                        }
                        savingWorker.ReportProgress(imagesSaved * 100 / cachesList.Count);
                    }
                    catch (WebException)
                    {
                        savingWorker.ReportProgress(imagesSaved * 100 / cachesList.Count, "Пропущена " + cache.Id);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка");
                    }
                    imagesSaved++;
                }
            }
            if (argument.PoiStyle)
            {
                savingWorker.ReportProgress(99, "Сохранение POI...");
                var poiPath = Path.Combine(argument.SelectedPath, "POI");
                Registry.SetValue("HKEY_CURRENT_USER\\SOFTWARE\\Garmin\\POI Loader\\Settings", "Directory", argument.SelectedPath);
                var psi = new System.Diagnostics.ProcessStartInfo(poiLoaderPath) { Arguments = "/s /d \"" + poiPath + "\"" };
                var result = System.Diagnostics.Process.Start(psi);
                result.WaitForExit();
                var regionFile = GetRegionFileName(argument.SelectedRegion);
                var regionPath = Path.Combine(poiPath, regionFile);
                regionPath = Path.ChangeExtension(regionPath, "gpi");
                if (File.Exists(regionPath))
                {
                    File.Delete(regionPath);
                }
                File.Move(Path.Combine(poiPath, "Poi.gpi"), regionPath);
                string[] sourceFiles = Directory.GetFiles(argument.SelectedPath, "*.*");
                foreach (var sourceFile in sourceFiles)
                {
                    File.Delete(sourceFile);
                }
            }
        }

        private void savingWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                lblCaches.Text = "Отменено!";
            }
            else if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message, "Ошибка");
                lblCaches.Text = "Ошибка!";
            }
            else
            {
                lblCaches.Text = "Готово!";
                btnSaveGpx.Text = "GPX";
                btnSavePoi.Text = "POI";
                btnSaveGpx.Enabled = true;
                btnSavePoi.Enabled = true;
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

        private void btnOptions_Click(object sender, EventArgs e)
        {
            new Options().ShowDialog();
        }
    }
}
