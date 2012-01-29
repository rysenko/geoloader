using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GeoLoader.Properties;

namespace GeoLoader
{
    public partial class Options : Form
    {
        public Options()
        {
            InitializeComponent();
        }

        private void Options_Load(object sender, EventArgs e)
        {
            numCacheExp.Value = Settings.Default.CacheExpirationInDays;
            numLogsInJournal.Value = Settings.Default.MaxLogEntriesToSave;
            checkSaveCachePhoto.Checked = Settings.Default.SaveCachePhotos;
            checkSaveTerritoryPhotos.Checked = Settings.Default.SaveTerritoryPhotos;
            checkTruncateDescriptions.Checked = Settings.Default.TruncateLongCacheDescriptions;
        }

        private void Options_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.CacheExpirationInDays = Convert.ToInt32(numCacheExp.Value);
            Settings.Default.MaxLogEntriesToSave = Convert.ToInt32(numLogsInJournal.Value);
            Settings.Default.SaveCachePhotos = checkSaveCachePhoto.Checked;
            Settings.Default.SaveTerritoryPhotos = checkSaveTerritoryPhotos.Checked;
            Settings.Default.TruncateLongCacheDescriptions = checkTruncateDescriptions.Checked;
            Settings.Default.Save();
            Settings.Default.Upgrade();
        }
    }
}
