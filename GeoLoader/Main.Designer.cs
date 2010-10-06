namespace GeoLoader
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.ddlCountry = new System.Windows.Forms.ComboBox();
            this.ddlRegion = new System.Windows.Forms.ComboBox();
            this.lblCaches = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.prgSaving = new System.Windows.Forms.ProgressBar();
            this.savingWorker = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // ddlCountry
            // 
            this.ddlCountry.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlCountry.FormattingEnabled = true;
            this.ddlCountry.Location = new System.Drawing.Point(12, 12);
            this.ddlCountry.Name = "ddlCountry";
            this.ddlCountry.Size = new System.Drawing.Size(146, 21);
            this.ddlCountry.TabIndex = 0;
            this.ddlCountry.SelectedIndexChanged += new System.EventHandler(this.ddlCountry_SelectedIndexChanged);
            // 
            // ddlRegion
            // 
            this.ddlRegion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlRegion.FormattingEnabled = true;
            this.ddlRegion.Location = new System.Drawing.Point(12, 39);
            this.ddlRegion.Name = "ddlRegion";
            this.ddlRegion.Size = new System.Drawing.Size(146, 21);
            this.ddlRegion.TabIndex = 1;
            this.ddlRegion.SelectedIndexChanged += new System.EventHandler(this.ddlRegion_SelectedIndexChanged);
            // 
            // lblCaches
            // 
            this.lblCaches.AutoSize = true;
            this.lblCaches.Location = new System.Drawing.Point(167, 17);
            this.lblCaches.Name = "lblCaches";
            this.lblCaches.Size = new System.Drawing.Size(41, 13);
            this.lblCaches.TabIndex = 2;
            this.lblCaches.Text = "Ready!";
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.Location = new System.Drawing.Point(168, 39);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(104, 21);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // prgSaving
            // 
            this.prgSaving.Location = new System.Drawing.Point(13, 67);
            this.prgSaving.Name = "prgSaving";
            this.prgSaving.Size = new System.Drawing.Size(259, 15);
            this.prgSaving.TabIndex = 4;
            // 
            // savingWorker
            // 
            this.savingWorker.WorkerReportsProgress = true;
            this.savingWorker.WorkerSupportsCancellation = true;
            this.savingWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.savingWorker_DoWork);
            this.savingWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.savingWorker_RunWorkerCompleted);
            this.savingWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.savingWorker_ProgressChanged);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 92);
            this.Controls.Add(this.prgSaving);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lblCaches);
            this.Controls.Add(this.ddlRegion);
            this.Controls.Add(this.ddlCountry);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GeoLoader";
            this.Load += new System.EventHandler(this.Main_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox ddlCountry;
        private System.Windows.Forms.ComboBox ddlRegion;
        private System.Windows.Forms.Label lblCaches;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ProgressBar prgSaving;
        private System.ComponentModel.BackgroundWorker savingWorker;


    }
}

