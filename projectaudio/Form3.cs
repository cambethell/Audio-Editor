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
//threaded dft in form4 and windowform performs faster than 
//the unthreaded dft done in form3. can be considered deprecated
//since many things don't work properly.
namespace projectaudio
{
    //form3 represents data in the frequency domain
    public partial class Form3 : Form
    {
        public Form3()
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
        //like form2, this will let you select a section of the frequency
        //data being represented in this form.
        private double[] selectedWave()
        {
            if (chart1.ChartAreas[0].CursorX.SelectionEnd < chart1.ChartAreas[0].CursorX.SelectionStart)
            {
                double temp = chart1.ChartAreas[0].CursorX.SelectionStart;
                chart1.ChartAreas[0].CursorX.SelectionStart = chart1.ChartAreas[0].CursorX.SelectionEnd;
                chart1.ChartAreas[0].CursorX.SelectionEnd = temp;
            }
            double[] selectedWaveData = new double[(int)chart1.ChartAreas[0].CursorX.SelectionEnd - (int)chart1.ChartAreas[0].CursorX.SelectionStart];
            for (int i = 0; i < selectedWaveData.Length; i++)
            {
                selectedWaveData[i] = dataWave[(int)chart1.ChartAreas[0].CursorX.SelectionStart + i];
            }
            return selectedWaveData;
        }
        //when the form loads, it will DFT the data it recieved and 
        //display it.
        private void Form3_Load(object sender, EventArgs e)
        {
            // clear data from the chart
            chart1.Series.Clear();

            // add an x-y series to the chart
            var xySeries = new Charting.Series()
            {
                LegendText = "Frequency",
                ChartType = Charting.SeriesChartType.Column,
                Color = Color.Brown,
            };
            chart1.Series.Add(xySeries);

            //define length of dft array
            double[] dataDFT = new double[dataWave.Length];
            //THIS DO THE DFT
            dataDFT = transform.DFT(dataWave);
            //this PRINTS the dft on the graph
            for (int i = 1; i < dataDFT.Length; i++)
            {
                //Print dft
                xySeries.Points.AddXY(i, dataDFT[i]);
                //just to check: print data!
                //xySeries.Points.AddXY(i, dataWave[i]);
            }
            chart1.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = Charting.ChartDashStyle.Dot;
            chart1.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = Charting.ChartDashStyle.Dot;
        }

        private void iDFT_Click(object sender, EventArgs e)
        {

        }
        //zoom functionality.
        private void zoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Charting.Axis XAXIS = chart1.ChartAreas[0].AxisX;
            XAXIS.ScaleView.Zoom(chart1.ChartAreas[0].CursorX.SelectionStart, chart1.ChartAreas[0].CursorX.SelectionEnd);
            chart1.ChartAreas[0].CursorX.SelectionStart = double.NaN;
            chart1.ChartAreas[0].CursorX.SelectionEnd = double.NaN;
        }
        //------- FILTERING STARTS HERE -----------//

        //this is the convolution function. it convolves the
        //new weights over the entire original data.
        private void convolver(double[] weights){
            for(int i=0;i<(dataWave.Length - weights.Length); i++) 
            {
                for (int j=0; j < weights.Length; j++)
                {
                    dataWave[i + j] = dataWave[i + j] * weights[j];
                    //if the data is all being reduced to 0's, this is done to make it clear
                    //that the data is very small. the graphs will auto-scale
                    //to the data given so it prevents very small numbers 
                    //from dominating the graph, which looks misleading.
                    if (dataWave[i + j] < 0.0001)
                    {
                        dataWave[i + j] = 0;
                    }
                }
            }
            //display the new convolved data in a form2, which displays
            //time domain data.
            Form2 newMDIChild = new Form2();
            newMDIChild.DataWave = dataWave;
            // Set the Parent Form of the Child window.
            newMDIChild.MdiParent = this.MdiParent;
            // Display the new form.
            newMDIChild.Show();
            this.Close();            
        }

        //this is the inverse DFT function. it takes in complex
        //values, which are either 0,0 or 1,1 in this instance of filtering.
        private double[] inverseDFT(complex[] values)
        {
            double[] dataSample = new double[values.Length];

            for (int t = 0; t < dataSample.Length; t++)
            {
                double sampleRe = 0;
                double sampleIm = 0;
                for (int f = 0; f < values.Length; f++)
                {
                    sampleRe += values[t].real * (Math.Cos(2 * Math.PI * f * t / values.Length));
                    sampleIm += values[t].imag * (Math.Sin(2 * Math.PI * f * t / values.Length));
                }
                //the datasample[t] value is simplified since we only need 
                //the real component of the idft for representing it again
                //in the time domain.
                dataSample[t] = Math.Sqrt(sampleRe * sampleRe + sampleIm * sampleIm) / values.Length;
            }
            return dataSample;
        }
        //this is the toolstrip menu item to select a low pass filter.
        private void lowPassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var filterWeights = new complex[dataWave.Length];
            for (int i = 0; i < filterWeights.Length/2; i++)
            {
                if (i < (int)chart1.ChartAreas[0].CursorX.SelectionStart)
                {
                    filterWeights[i] = new complex(1, 1);
                    filterWeights[filterWeights.Length - i - 1] = new complex(1, 1);
                }
                else
                {
                    filterWeights[i] = new complex(1, 1);
                    filterWeights[filterWeights.Length - i - 1] = new complex(0, 1);
                }
            }
            //after the 1-0 mask is designed, it will iDFT it, then convolve it
            //across the original data and finally send it to a form2 to be displayed.
            convolver(inverseDFT(filterWeights));
        }
        //the high pass
        private void highPassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            complex[] filterWeights = new complex[dataWave.Length];
            for (int i = (int)chart1.ChartAreas[0].CursorX.SelectionStart; i < filterWeights.Length/2; i++)
            {
                filterWeights[i] = new complex(0, 0);
                filterWeights[filterWeights.Length - i - 1] = new complex(0, 0);
            }
            //after the 1-0 mask is designed, it will iDFT it, then convolve it
            //across the original data and finally send it to a form2 to be displayed.
            convolver(inverseDFT(filterWeights));
        }

        private void bandPassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            complex[] filterWeights = new complex[dataWave.Length];
            for (int i = (int)chart1.ChartAreas[0].CursorX.SelectionStart; i < (int)chart1.ChartAreas[0].CursorX.SelectionEnd; i++)
            {
                filterWeights[i] = new complex(0, 0);
                filterWeights[filterWeights.Length - i - 1] = new complex(0, 0);
            }
            //after the 1-0 mask is designed, it will iDFT it, then convolve it
            //across the original data and finally send it to a form2 to be displayed.
            convolver(inverseDFT(filterWeights));
        }

    }
}
