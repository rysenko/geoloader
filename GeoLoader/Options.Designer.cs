namespace GeoLoader
{
    partial class Options
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Options));
            this.numCacheExp = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.checkSaveCachePhoto = new System.Windows.Forms.CheckBox();
            this.checkTruncateDescriptions = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.numLogsInJournal = new System.Windows.Forms.NumericUpDown();
            this.checkSaveTerritoryPhotos = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numCacheExp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLogsInJournal)).BeginInit();
            this.SuspendLayout();
            // 
            // numCacheExp
            // 
            this.numCacheExp.Location = new System.Drawing.Point(12, 107);
            this.numCacheExp.Name = "numCacheExp";
            this.numCacheExp.Size = new System.Drawing.Size(48, 20);
            this.numCacheExp.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(66, 109);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(196, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Время жизни кэша запросов (в днях)";
            // 
            // checkSaveCachePhoto
            // 
            this.checkSaveCachePhoto.AutoSize = true;
            this.checkSaveCachePhoto.Location = new System.Drawing.Point(12, 12);
            this.checkSaveCachePhoto.Name = "checkSaveCachePhoto";
            this.checkSaveCachePhoto.Size = new System.Drawing.Size(190, 17);
            this.checkSaveCachePhoto.TabIndex = 0;
            this.checkSaveCachePhoto.Text = "Сохранять фотографию тайника";
            this.checkSaveCachePhoto.UseVisualStyleBackColor = true;
            // 
            // checkTruncateDescriptions
            // 
            this.checkTruncateDescriptions.AutoSize = true;
            this.checkTruncateDescriptions.Location = new System.Drawing.Point(12, 58);
            this.checkTruncateDescriptions.Name = "checkTruncateDescriptions";
            this.checkTruncateDescriptions.Size = new System.Drawing.Size(260, 17);
            this.checkTruncateDescriptions.TabIndex = 2;
            this.checkTruncateDescriptions.Text = "Обрезать длинные описания (до 8192 знаков)";
            this.checkTruncateDescriptions.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(66, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(226, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Количество записей в журнале посещений";
            // 
            // numLogsInJournal
            // 
            this.numLogsInJournal.Location = new System.Drawing.Point(12, 81);
            this.numLogsInJournal.Name = "numLogsInJournal";
            this.numLogsInJournal.Size = new System.Drawing.Size(48, 20);
            this.numLogsInJournal.TabIndex = 3;
            // 
            // checkSaveTerritoryPhotos
            // 
            this.checkSaveTerritoryPhotos.AutoSize = true;
            this.checkSaveTerritoryPhotos.Location = new System.Drawing.Point(12, 35);
            this.checkSaveTerritoryPhotos.Name = "checkSaveTerritoryPhotos";
            this.checkSaveTerritoryPhotos.Size = new System.Drawing.Size(201, 17);
            this.checkSaveTerritoryPhotos.TabIndex = 1;
            this.checkSaveTerritoryPhotos.Text = "Сохранять фотографии местности";
            this.checkSaveTerritoryPhotos.UseVisualStyleBackColor = true;
            // 
            // Options
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(303, 139);
            this.Controls.Add(this.checkSaveTerritoryPhotos);
            this.Controls.Add(this.numLogsInJournal);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkTruncateDescriptions);
            this.Controls.Add(this.checkSaveCachePhoto);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numCacheExp);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Options";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Настройки";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Options_FormClosing);
            this.Load += new System.EventHandler(this.Options_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numCacheExp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLogsInJournal)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numCacheExp;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkSaveCachePhoto;
        private System.Windows.Forms.CheckBox checkTruncateDescriptions;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numLogsInJournal;
        private System.Windows.Forms.CheckBox checkSaveTerritoryPhotos;
    }
}