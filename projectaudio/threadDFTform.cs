using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Charting = System.Windows.Forms.DataVisualization.Charting;

//this form is used almost excusively for displaying that the
//threaded dft performs faster than the unthreaded dft done in
//form3.
namespace projectaudio
{
    public partial class threadDFTform : Form
    {
        public threadDFTform()
        {
            InitializeComponent();
        }
        private double[] dataWave;
        //initialize some DataWave to pass data to on window creation
        public double[] DataWave
        {
            get { return dataWave; }
            set { dataWave = value; }
        }

        private void threadDFTform_Load(object sender, EventArgs e)
        {
            chart1.Series.Clear();
            var xySeries = new Charting.Series()
            {
                LegendText = "Frequency",
                ChartType = Charting.SeriesChartType.Column,
                Color = Color.Green,
            };
            chart1.Series.Add(xySeries);
            //define length of dft array
            double[] dataDFT = new double[dataWave.Length];
            //THIS DO THE DFT
            dataDFT = transform.threadDFT(dataWave);

            for (int i = 1; i < dataDFT.Length; i++)
            {
                xySeries.Points.AddXY(i, dataDFT[i]);
            }
            chart1.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = Charting.ChartDashStyle.Dot;
            chart1.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = Charting.ChartDashStyle.Dot;
        }
    }
}
