using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using Charting = System.Windows.Forms.DataVisualization.Charting;
using System.Runtime.InteropServices;

namespace projectaudio
{
    public partial class Form2 : Form
    {
        //setting up the initial data members for the form
        private double[] dataWave;
        private byte[] byteStuff;
        private waveHead._wave_file_hdr_ wavhdrW;
        private recordDataPtr.recordData recData;
        public Form2()
        {
            InitializeComponent();
        }
        //importing the dll's for playing sound
        //sendplay sends a pointer to the allocated bytes, the size of it,
        //and also sends the relevant wave header info like sample rate
        //and bit depth.
        [DllImport("winmm.dll")] public static extern int mmioStringToFOURCC([MarshalAs(UnmanagedType.LPStr)] String s, int flags);

        [DllImport("recordaudio.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sendPlay(IntPtr pdata, int size, int sampleRate, short bitDepth);

        [DllImport("recordaudio.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sendPlayStop();
        //this function returns the selection on the graph as a new double array. this
        //is mostly useful for the cutting, copying, and pasting.
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
        //the getting and setting parts for passing around data between child windows
        public recordDataPtr.recordData RecData
        {
            get { return recData; }
            set { recData = value; }
        }

        public double[] DataWave
        {
            get { return dataWave; }
            set { dataWave = value; }
        }
        
        public waveHead._wave_file_hdr_ WaveHeader
        {
            get { return wavhdrW; }
            set { wavhdrW = value; }
        }

        public byte[] ByteStuff
        {
            get { return byteStuff; }
            set { byteStuff = value; }
        }
        //when form2 loads, it will display the audio samples in the time domain
        private void Form2_Load(object sender, EventArgs e)
        {
            chart1.Series.Clear();
            var xySeries = new Charting.Series()
            {
                LegendText = "Frequency",
                ChartType = Charting.SeriesChartType.Line,
                Color = Color.Brown,
            };
            chart1.Series.Add(xySeries);

            for (int i = 0; i < dataWave.Length; i++)
            {
                xySeries.Points.AddXY(i, dataWave[i]);
            }
            chart1.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = Charting.ChartDashStyle.Dot;
            chart1.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = Charting.ChartDashStyle.Dot;
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }
        //this is the cut button that will cut out a section of the graph
        //and store it in the clipboard. chopwaveformat is the name given
        //to represent an array of doubles.
        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetData("ChopWaveFormat", selectedWave());
            double[] tempData;
            tempData = (double[])Clipboard.GetData("ChopWaveFormat");
            double[] newCutWave = new double[dataWave.Length - tempData.Length];
            for (int i = 0; i < (newCutWave.Length); i++)
            {
                if (i < (int)chart1.ChartAreas[0].CursorX.SelectionStart)
                {
                    newCutWave[i] = dataWave[i];
                } else
                {
                    newCutWave[i] = dataWave[i + tempData.Length];
                }
            }
            //reset the selection cursor
            chart1.ChartAreas[0].CursorX.SelectionStart = double.NaN;
            chart1.ChartAreas[0].CursorX.SelectionEnd = double.NaN;
            dataWave = newCutWave;
            //plotting the new wave missing the cut piece
            chart1.Series.Clear();
            var xySeries = new Charting.Series()
            {
                LegendText = "Frequency",
                ChartType = Charting.SeriesChartType.Line,
                Color = Color.Brown,
            };
            chart1.Series.Add(xySeries);
            for (int i = 0; i < newCutWave.Length; i++)
            {
                xySeries.Points.AddXY(i, newCutWave[i]);
            }
            chart1.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = Charting.ChartDashStyle.Dot;
            chart1.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = Charting.ChartDashStyle.Dot;
        }
        //this is the copy function to copy a selection of the graph data
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetData("ChopWaveFormat", selectedWave());
        }
        //this is the paste function to paste from the clipboard onto a
        //graph. this can be done between different files, but does not 
        //correctly handle up/down sampling between differing sample rates.
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsData("ChopWaveFormat"))
            {   
                double[] tempData;
                tempData = (double[])Clipboard.GetData("ChopWaveFormat");
                double[] newCutWave = new double[dataWave.Length + tempData.Length];
                //does not handle pasting and replacing a selection
                //will only past at the start selection
                for (int i = 0; i < dataWave.Length; i++)
                {
                    if (i < (int)chart1.ChartAreas[0].CursorX.SelectionStart)
                    {
                        newCutWave[i] = dataWave[i];
                    }
                    if (i == (int)chart1.ChartAreas[0].CursorX.SelectionStart)
                    {
                        for (int j = 0; j < tempData.Length; j++)
                        {
                            newCutWave[j + i] = tempData[j];
                        }
                    }
                    if (i > (int)chart1.ChartAreas[0].CursorX.SelectionStart)
                    {
                        newCutWave[i + tempData.Length] = dataWave[i];
                    }
                }
                dataWave = newCutWave;
                chart1.ChartAreas[0].CursorX.SelectionStart = double.NaN;
                chart1.ChartAreas[0].CursorX.SelectionEnd = double.NaN;
                //plotting the new wave with the extra clipboard piece
                chart1.Series.Clear();
                var xySeries = new Charting.Series()
                {
                    LegendText = "Frequency",
                    ChartType = Charting.SeriesChartType.Line,
                    Color = Color.Brown,
                };
                chart1.Series.Add(xySeries);
                for (int i = 0; i < newCutWave.Length; i++)
                {
                    xySeries.Points.AddXY(i, newCutWave[i]);
                }
                chart1.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = Charting.ChartDashStyle.Dot;
                chart1.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = Charting.ChartDashStyle.Dot;
            }
        }
        //this is the button that will call the form3, which computes and
        //displays the unthreaded dft of the entire sample data.
        private void DFTButton_Click(object sender, EventArgs e)
        {
            Form3 newMDIChild = new Form3();
            newMDIChild.DataWave = dataWave;
            // Set the Parent Form of the Child window.
            newMDIChild.MdiParent = this.MdiParent;
            // Display the new form.
            newMDIChild.Show();
        }
        //this is the save button that will save a file. it dynamically reads
        //the wave header information of the current data being used in the form2.
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            short[] samples2Int = dataWave.Select(x => (short)(x)).ToArray();
            byte[] waveByteData = samples2Int.Select(x => Convert.ToInt16(x)).SelectMany(x => BitConverter.GetBytes(x)).ToArray();
            //setheader bytes to be added
            wavhdrW.RIFF = mmioStringToFOURCC("RIFF", 0);
            wavhdrW.WAVE = mmioStringToFOURCC("WAVE", 0);
            wavhdrW.fmt_ = mmioStringToFOURCC("fmt ", 0);
            wavhdrW.fmt_size = 16;
            wavhdrW.format_tag = 1;
            wavhdrW.nchannels = 1;
            wavhdrW.avg_bytes_per_sec = (short)(wavhdrW.samples_per_sec * wavhdrW.bits_per_sample * wavhdrW.nchannels / 8);
            wavhdrW.nblock_align = (short)(wavhdrW.nchannels * wavhdrW.bits_per_sample / 8);
            wavhdrW.data = mmioStringToFOURCC("data", 0); ;
            wavhdrW.data_size = (uint)(dataWave.Length * wavhdrW.bits_per_sample/8);
            wavhdrW.filesize_minus_4 = (int)(wavhdrW.data_size + 36);
            //saving the file
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "WAV|*.wav";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                //using a binary writer to write the bytes in the file
                using (Stream s = File.Open(sfd.FileName, FileMode.CreateNew))
                using (BinaryWriter bw = new BinaryWriter(s))
                {
                    bw.Write(wavhdrW.RIFF);
                    bw.Write(wavhdrW.filesize_minus_4);
                    bw.Write(wavhdrW.WAVE);
                    bw.Write(wavhdrW.fmt_);
                    bw.Write(wavhdrW.fmt_size);
                    bw.Write(wavhdrW.format_tag);
                    bw.Write(wavhdrW.nchannels);
                    bw.Write(wavhdrW.samples_per_sec);
                    bw.Write(wavhdrW.avg_bytes_per_sec);
                    bw.Write(wavhdrW.nblock_align);
                    bw.Write(wavhdrW.bits_per_sample);
                    bw.Write(wavhdrW.data);
                    bw.Write(wavhdrW.data_size);
                    bw.Write(waveByteData);
                }
            }
        }
        //this is the blackman harris windowing function that will do a windowed
        //dft on the selection of the data.
        private void blackmanHarrisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double[] windowWave = WindowingClass.BlackmanHarrisWindow(selectedWave());
            double[] sendDataWave = new double[dataWave.Length];
            for (int i = 0; i < dataWave.Length; i++)
            {
                sendDataWave[i] = dataWave[i];
            }
            WindowForm newMDIChild = new WindowForm();

            newMDIChild.DataWave = sendDataWave;
            newMDIChild.WindowWave = windowWave;
            // Set the Parent Form of the Child window.
            newMDIChild.MdiParent = this.MdiParent;
            newMDIChild.Show();
        }
        //this is the hamming windowing function that will do a windowed
        //dft on the selection of the data.
        private void hammingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double[] windowWave = WindowingClass.HammingWindow(selectedWave());
            double[] sendDataWave = new double[dataWave.Length];
            for (int i = 0; i < dataWave.Length; i++)
            {
                sendDataWave[i] = dataWave[i];
            }
            WindowForm newMDIChild = new WindowForm();
            newMDIChild.WaveHeader = wavhdrW;
            newMDIChild.DataWave = sendDataWave;
            newMDIChild.WindowWave = windowWave;
            // Set the Parent Form of the Child window.
            newMDIChild.MdiParent = this.MdiParent;
            // Display the new form.
            newMDIChild.Show();
        }
        //this is the square windowing function that will do a windowed
        //dft on the selection of the data. this is effectively the worst
        //windowing option.
        private void squareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double[] windowWave = selectedWave();
            double[] sendDataWave = new double[dataWave.Length];
            for (int i = 0; i < dataWave.Length; i++)
            {
                sendDataWave[i] = dataWave[i];
            }
            WindowForm newMDIChild = new WindowForm();
            newMDIChild.DataWave = sendDataWave;
            newMDIChild.WindowWave = windowWave;
            // Set the Parent Form of the Child window.
            newMDIChild.MdiParent = this.MdiParent;
            // Display the new form.
            newMDIChild.Show();
        }
        //this is the triangle windowing function that will do a windowed
        //dft on the selection of the data.
        private void triangleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double[] windowWave = WindowingClass.TriangleWindow(selectedWave());
            double[] sendDataWave = new double[dataWave.Length];
            for (int i = 0; i < dataWave.Length; i++)
            {
                sendDataWave[i] = dataWave[i];
            }
            WindowForm newMDIChild = new WindowForm();
            newMDIChild.DataWave = sendDataWave;
            newMDIChild.WindowWave = windowWave;
            // Set the Parent Form of the Child window.
            newMDIChild.MdiParent = this.MdiParent;
            // Display the new form.
            newMDIChild.Show();
        }
        //this is the zoom button that allows you to zoom in on a particular
        //section of the data.
        private void zoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Charting.Axis XAXIS = chart1.ChartAreas[0].AxisX;
            XAXIS.ScaleView.Zoom(chart1.ChartAreas[0].CursorX.SelectionStart, chart1.ChartAreas[0].CursorX.SelectionEnd);
            chart1.ChartAreas[0].CursorX.SelectionStart = double.NaN;
            chart1.ChartAreas[0].CursorX.SelectionEnd = double.NaN;
        }
        //this is the play button. it will first check the bit depth
        //and then convert the sample data, which is depicted in doubles,
        //and convert it to bytes and marshall it. it then passes the pointer
        //to the win32 DLL via the sendPlay function, along with the relevant
        //waveheader info like sample rate and bit depth.
        private void playToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (wavhdrW.bits_per_sample == 16)
            {
                short[] samples2Int;
                byte[] waveByteData = new byte[] { 0 };
                samples2Int = dataWave.Select(x => (short)(x)).ToArray();
                waveByteData = samples2Int.Select(x => Convert.ToInt16(x)).SelectMany(x => BitConverter.GetBytes(x)).ToArray();
                IntPtr pSamples = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(byte)) * waveByteData.Length);
                Marshal.Copy(waveByteData, 0, pSamples, waveByteData.Length);
                sendPlay(pSamples, waveByteData.Length, wavhdrW.samples_per_sec, wavhdrW.bits_per_sample);
                Marshal.FreeHGlobal(pSamples);
            } else if (wavhdrW.bits_per_sample == 32)
            {
                int[] samples2Int;
                byte[] waveByteData = new byte[] { 0 };
                samples2Int = dataWave.Select(x => (int)(x)).ToArray();
                waveByteData = samples2Int.Select(x => Convert.ToInt32(x)).SelectMany(x => BitConverter.GetBytes(x)).ToArray();
                IntPtr pSamples = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(byte)) * waveByteData.Length);
                Marshal.Copy(waveByteData, 0, pSamples, waveByteData.Length);
                sendPlay(pSamples, waveByteData.Length, wavhdrW.samples_per_sec, wavhdrW.bits_per_sample);
                Marshal.FreeHGlobal(pSamples);
            }
        }
        //this is the button that open a threadDFTform and do a threaded
        //dft on the entire sample data. this is mostly useful for comparing
        //the effectivenwess of threading against the non-threaded dft. 
        private void threadDFTButton_Click(object sender, EventArgs e)
        {
            threadDFTform newMDIChild3 = new threadDFTform();
            newMDIChild3.DataWave = dataWave;
            newMDIChild3.MdiParent = this.MdiParent;
            newMDIChild3.Show();
        }
    }
}
