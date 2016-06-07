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
//this is the form that is used most often to display the
//data as frequency. allows filtering and zooming.
namespace projectaudio
{
    public partial class WindowForm : Form
    {
        public int windowtype = 0;

        public WindowForm()
        {
            InitializeComponent();
        }

        private waveHead._wave_file_hdr_ wavhdrW;

        public waveHead._wave_file_hdr_ WaveHeader
        {
            get { return wavhdrW; }
            set { wavhdrW = value; }
        }
        private double[] windowWave;
        //initialize some DataWave to pass data to on window creation
        public double[] WindowWave
        {
            get { return windowWave; }
            set { windowWave = value; }
        }

        private double[] dataWave;
        //initialize some DataWave to pass data to on window creation
        public double[] DataWave
        {
            get { return dataWave; }
            set { dataWave = value; }
        }
        //this is the same selection function for grabbing
        //a section of the graph as in form2.
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
                selectedWaveData[i] = windowWave[(int)chart1.ChartAreas[0].CursorX.SelectionStart + i];
            }
            return selectedWaveData;
        }
        //when the form loads it will do a threaded
        //dft on the data and display it.
        private void WindowForm_Load_1(object sender, EventArgs e)
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
            double[] dataDFT = new double[windowWave.Length];
            dataDFT = transform.threadDFT(windowWave);

            //this PRINTS the dft on the graph
            for (int i = 1; i < dataDFT.Length; i++)
            {
                //Print dft
                xySeries.Points.AddXY(i, dataDFT[i]);
                //just to check: print data!
            }

            // set the axis
            chart1.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = Charting.ChartDashStyle.Dot;
            chart1.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = Charting.ChartDashStyle.Dot;
        }
        //this is the same zoom functionality as in the form2
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
        private void convolver(double[] weights)
        {
            for (int N = 0; N < (dataWave.Length); N++)
            {
                double Conv = 0;
                double sampleValue = 0;
                for (int w = 0; w < weights.Length; w++)
                {
                    sampleValue = 0;
                    if ((N + w) < (dataWave.Length - 1))
                    {
                        sampleValue = dataWave[N + w];
                    }
                    Conv += sampleValue * weights[w];   
                }
                dataWave[N] = Conv;
            }
            //display the new convolved data in a form2, which displays
            //time domain data.
            Form2 newMDIChild = new Form2();
            newMDIChild.DataWave = dataWave;
            newMDIChild.WaveHeader = wavhdrW;
            // Set the Parent Form of the Child window.
            newMDIChild.MdiParent = this.MdiParent;
            // Display the new form.
            newMDIChild.Show();
            this.Close();
        }
        //this is the toolstrip menu item to select a low pass filter.
        private void lowPassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //this is where the 1-0 mask is designed.
            var filterWeights = new complex[windowWave.Length];
            if((windowWave.Length%2)!=0)
            {
                filterWeights[(filterWeights.Length / 2)] = new complex(0, 0);
            }
            for (int i = 0; i < filterWeights.Length / 2; i++)
            {
                if (i < (int)chart1.ChartAreas[0].CursorX.SelectionStart)
                {
                    filterWeights[i] = new complex(1, 1);
                    filterWeights[filterWeights.Length - i - 1] = new complex(1, 1);
                }
                else
                {
                    filterWeights[i] = new complex(0, 0);
                    filterWeights[filterWeights.Length - i - 1] = new complex(0, 0);
                }
            }
            //after the 1-0 mask is designed, it will iDFT it, window it,
            //then convolve it across the original data and finally send 
            //it to a form2 to be displayed.
            if (windowtype == 1)
            {
                convolver(WindowingClass.TriangleWindow(transform.inverseDFT(filterWeights)));
            }
            else if (windowtype == 2)
            {
                convolver(WindowingClass.BlackmanHarrisWindow(transform.inverseDFT(filterWeights)));
            }
            else if (windowtype == 3)
            {
                convolver(WindowingClass.HammingWindow(transform.inverseDFT(filterWeights)));
            }
            else
            {
                convolver(transform.inverseDFT(filterWeights));
            }
        }

        private void bandPassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //this is where the 1-0 mask is designed.
            var filterWeights = new complex[windowWave.Length];
            if ((windowWave.Length % 2) != 0)
            {
                filterWeights[(filterWeights.Length / 2)] = new complex(0, 0);
            }
            for (int i = 0; i < filterWeights.Length / 2; i++)
            {
                if (i > (int)chart1.ChartAreas[0].CursorX.SelectionStart && i < (int)chart1.ChartAreas[0].CursorX.SelectionEnd)
                {
                    filterWeights[i] = new complex(1, 1);
                    filterWeights[filterWeights.Length - i - 1] = new complex(1, 1);
                }
                else
                {
                    filterWeights[i] = new complex(0, 0);
                    filterWeights[filterWeights.Length - i - 1] = new complex(0, 0);
                }
            }
            //after the 1-0 mask is designed, it will iDFT it, window it,
            //then convolve it across the original data and finally send 
            //it to a form2 to be displayed.
            if (windowtype == 1)
            {
                convolver(WindowingClass.TriangleWindow(transform.inverseDFT(filterWeights)));
            }
            else if (windowtype == 2)
            {
                convolver(WindowingClass.BlackmanHarrisWindow(transform.inverseDFT(filterWeights)));
            }
            else if (windowtype == 3)
            {
                convolver(WindowingClass.HammingWindow(transform.inverseDFT(filterWeights)));
            }
            else
            {
                convolver(transform.inverseDFT(filterWeights));
            }
        }
        
        private void highPassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //this is where the 1-0 mask is designed.
            var filterWeights = new complex[windowWave.Length];
            if ((windowWave.Length % 2) != 0)
            {
                filterWeights[(filterWeights.Length / 2)] = new complex(1, 1);
            }
            for (int i = 0; i < filterWeights.Length / 2; i++)
            {
                if (i > (int)chart1.ChartAreas[0].CursorX.SelectionStart)
                {
                    filterWeights[i] = new complex(1, 1);
                    filterWeights[filterWeights.Length - i - 1] = new complex(1, 1);
                }
                else
                {
                    filterWeights[i] = new complex(0, 0);
                    filterWeights[filterWeights.Length - i - 1] = new complex(0, 0);
                }
            }
            //after the 1-0 mask is designed, it will iDFT it, window it,
            //then convolve it across the original data and finally send 
            //it to a form2 to be displayed.
            if (windowtype==1)
            {
                convolver(WindowingClass.TriangleWindow(transform.inverseDFT(filterWeights)));
            } else if(windowtype==2)
            {
                convolver(WindowingClass.BlackmanHarrisWindow(transform.inverseDFT(filterWeights)));
            } else if(windowtype==3)
            {
                convolver(WindowingClass.HammingWindow(transform.inverseDFT(filterWeights)));
            } else
            {
                convolver(transform.inverseDFT(filterWeights));
            }
        }

        private void blackmanHarrisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in applyWindowingToolStripMenuItem.DropDownItems)
            {
                item.Checked = false;
            }
            blackmanHarrisToolStripMenuItem.Checked = true;
            windowtype = 2;
        }
        //this is the toolstrip menu item that lets you select which type of 
        //you want done on the data.
        private void triangleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in applyWindowingToolStripMenuItem.DropDownItems)
            {
                item.Checked = false;
            }
            triangleToolStripMenuItem.Checked = true;
            windowtype = 1;
        }

        private void hammingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in applyWindowingToolStripMenuItem.DropDownItems)
            {
                item.Checked = false;
            }
            hammingToolStripMenuItem.Checked = true;
            windowtype = 3;
        }

        private void noneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in applyWindowingToolStripMenuItem.DropDownItems)
            {
                item.Checked = false;
            }
           noneToolStripMenuItem.Checked = true;
            windowtype = 0;
        }
    }
}
