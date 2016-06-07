namespace projectaudio
{
    partial class WindowForm
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea5 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend5 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.zoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filteringToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lowPassToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bandPassToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.highPassToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.applyWindowingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.triangleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.blackmanHarrisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hammingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chart1
            // 
            this.chart1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea5.AxisX.ScaleView.Zoomable = false;
            chartArea5.CursorX.IsUserEnabled = true;
            chartArea5.CursorX.IsUserSelectionEnabled = true;
            chartArea5.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea5);
            legend5.Name = "Legend1";
            this.chart1.Legends.Add(legend5);
            this.chart1.Location = new System.Drawing.Point(-1, 33);
            this.chart1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chart1.Name = "chart1";
            series5.ChartArea = "ChartArea1";
            series5.Legend = "Legend1";
            series5.Name = "Frequency";
            this.chart1.Series.Add(series5);
            this.chart1.Size = new System.Drawing.Size(1344, 414);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.AllowMerge = false;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zoomToolStripMenuItem,
            this.filteringToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1339, 28);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // zoomToolStripMenuItem
            // 
            this.zoomToolStripMenuItem.Name = "zoomToolStripMenuItem";
            this.zoomToolStripMenuItem.Size = new System.Drawing.Size(61, 24);
            this.zoomToolStripMenuItem.Text = "&Zoom";
            this.zoomToolStripMenuItem.Click += new System.EventHandler(this.zoomToolStripMenuItem_Click);
            // 
            // filteringToolStripMenuItem
            // 
            this.filteringToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lowPassToolStripMenuItem,
            this.bandPassToolStripMenuItem,
            this.highPassToolStripMenuItem,
            this.applyWindowingToolStripMenuItem});
            this.filteringToolStripMenuItem.Name = "filteringToolStripMenuItem";
            this.filteringToolStripMenuItem.Size = new System.Drawing.Size(84, 24);
            this.filteringToolStripMenuItem.Text = "&Filtering...";
            // 
            // lowPassToolStripMenuItem
            // 
            this.lowPassToolStripMenuItem.Name = "lowPassToolStripMenuItem";
            this.lowPassToolStripMenuItem.Size = new System.Drawing.Size(212, 26);
            this.lowPassToolStripMenuItem.Text = "&Low Pass";
            this.lowPassToolStripMenuItem.Click += new System.EventHandler(this.lowPassToolStripMenuItem_Click);
            // 
            // bandPassToolStripMenuItem
            // 
            this.bandPassToolStripMenuItem.Name = "bandPassToolStripMenuItem";
            this.bandPassToolStripMenuItem.Size = new System.Drawing.Size(212, 26);
            this.bandPassToolStripMenuItem.Text = "&Band Pass";
            this.bandPassToolStripMenuItem.Click += new System.EventHandler(this.bandPassToolStripMenuItem_Click);
            // 
            // highPassToolStripMenuItem
            // 
            this.highPassToolStripMenuItem.Name = "highPassToolStripMenuItem";
            this.highPassToolStripMenuItem.Size = new System.Drawing.Size(212, 26);
            this.highPassToolStripMenuItem.Text = "&High Pass";
            this.highPassToolStripMenuItem.Click += new System.EventHandler(this.highPassToolStripMenuItem_Click);
            // 
            // applyWindowingToolStripMenuItem
            // 
            this.applyWindowingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.triangleToolStripMenuItem,
            this.blackmanHarrisToolStripMenuItem,
            this.hammingToolStripMenuItem,
            this.noneToolStripMenuItem});
            this.applyWindowingToolStripMenuItem.Name = "applyWindowingToolStripMenuItem";
            this.applyWindowingToolStripMenuItem.Size = new System.Drawing.Size(212, 26);
            this.applyWindowingToolStripMenuItem.Text = "Apply &Windowing...";
            // 
            // triangleToolStripMenuItem
            // 
            this.triangleToolStripMenuItem.CheckOnClick = true;
            this.triangleToolStripMenuItem.Name = "triangleToolStripMenuItem";
            this.triangleToolStripMenuItem.Size = new System.Drawing.Size(193, 26);
            this.triangleToolStripMenuItem.Text = "Triangle";
            this.triangleToolStripMenuItem.Click += new System.EventHandler(this.triangleToolStripMenuItem_Click);
            // 
            // blackmanHarrisToolStripMenuItem
            // 
            this.blackmanHarrisToolStripMenuItem.CheckOnClick = true;
            this.blackmanHarrisToolStripMenuItem.Name = "blackmanHarrisToolStripMenuItem";
            this.blackmanHarrisToolStripMenuItem.Size = new System.Drawing.Size(193, 26);
            this.blackmanHarrisToolStripMenuItem.Text = "Blackman-Harris";
            this.blackmanHarrisToolStripMenuItem.Click += new System.EventHandler(this.blackmanHarrisToolStripMenuItem_Click);
            // 
            // hammingToolStripMenuItem
            // 
            this.hammingToolStripMenuItem.CheckOnClick = true;
            this.hammingToolStripMenuItem.Name = "hammingToolStripMenuItem";
            this.hammingToolStripMenuItem.Size = new System.Drawing.Size(193, 26);
            this.hammingToolStripMenuItem.Text = "Hamming";
            this.hammingToolStripMenuItem.Click += new System.EventHandler(this.hammingToolStripMenuItem_Click);
            // 
            // noneToolStripMenuItem
            // 
            this.noneToolStripMenuItem.Checked = true;
            this.noneToolStripMenuItem.CheckOnClick = true;
            this.noneToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.noneToolStripMenuItem.Name = "noneToolStripMenuItem";
            this.noneToolStripMenuItem.Size = new System.Drawing.Size(193, 26);
            this.noneToolStripMenuItem.Text = "None";
            this.noneToolStripMenuItem.Click += new System.EventHandler(this.noneToolStripMenuItem_Click);
            // 
            // WindowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1339, 446);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "WindowForm";
            this.Text = "Windowed Signal";
            this.Load += new System.EventHandler(this.WindowForm_Load_1);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem zoomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filteringToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lowPassToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bandPassToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem highPassToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem applyWindowingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem triangleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem blackmanHarrisToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hammingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem noneToolStripMenuItem;
    }
}