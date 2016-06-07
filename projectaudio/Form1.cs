using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Charting = System.Windows.Forms.DataVisualization.Charting;
using System.Runtime.InteropServices;
using System.Diagnostics;
//this is the parent window to the project.
namespace projectaudio
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //Load in my DLL file functionality for recording and playing
        //the endDialog function returns the pointer to the byte data
        //after recording is finished.
        //the startRec function passes the sample rate and bit depth
        //info to the play function in the win32 DLL.
        [DllImport("winmm.dll")]
        public static extern int mmioStringToFOURCC([MarshalAs(UnmanagedType.LPStr)] String s, int flags);

        [DllImport("recordaudio.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern Boolean createDialog();

        [DllImport("recordaudio.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern recordDataPtr.recordData endDialog();

        [DllImport("recordaudio.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int startRec(int sampleRate, short bitDepth);

        [DllImport("recordaudio.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int stopRec();

        Form4 recordAlert = new Form4();

        waveHead._wave_file_hdr_ wavhdr;

        double[] graphicalSamples;
        byte[] audibleBytes;
        //Function that opens a file and puts the wav file data into a byte array and returns it
        private byte[] parseWAV(OpenFileDialog ofd)
        {
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                toolStripTextBox1.Text = ofd.FileName;
                toolStripTextBox2.Text = ofd.SafeFileName;
                byte[] allBytesA = File.ReadAllBytes(ofd.FileName);
                
                //parse file header here!
                wavhdr.RIFF = BitConverter.ToInt32(allBytesA, 0);
                wavhdr.filesize_minus_4 = BitConverter.ToInt32(allBytesA, 4);
                wavhdr.WAVE = BitConverter.ToInt32(allBytesA, 8);
                wavhdr.fmt_ = BitConverter.ToInt32(allBytesA, 12);
                wavhdr.fmt_size = BitConverter.ToInt32(allBytesA, 16);
                wavhdr.format_tag = BitConverter.ToInt16(allBytesA, 20);
                wavhdr.nchannels = BitConverter.ToInt16(allBytesA, 22);
                wavhdr.samples_per_sec = BitConverter.ToInt32(allBytesA, 24);
                wavhdr.avg_bytes_per_sec = BitConverter.ToInt32(allBytesA, 28);
                wavhdr.nblock_align = BitConverter.ToInt16(allBytesA, 32);
                wavhdr.bits_per_sample = BitConverter.ToInt16(allBytesA, 34);
                wavhdr.data = BitConverter.ToInt32(allBytesA, 36);
                wavhdr.data_size = BitConverter.ToUInt32(allBytesA, 40);
                
                return allBytesA;
            } else
            {
                byte[] fudged = new byte[] {0};
                return fudged;
            }
        }
        //This function calls the above function when you click to open a file.
        //it gets the file as a byte array from parseWave(), and passes the data to 
        //a child window to display it
        private void insertFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "WAV|*.wav";
            byte[] bytesToListen = parseWAV(ofd);
            if(bytesToListen.Length == 1)
            {
                return;
            }

            //this is the if statement that handles 32bit depth and 16 bit depth
            if (wavhdr.bits_per_sample == 32)
            {
                int[] bytes2Samples = new int[wavhdr.data_size / wavhdr.nblock_align];
                for (int M = 0; M < (wavhdr.data_size / wavhdr.nblock_align); M++)
                {
                    bytes2Samples[M] = BitConverter.ToInt32(bytesToListen, 44 + M * wavhdr.nblock_align);
                }
                double[] dubbleSamples = bytes2Samples.Select(x => (double)(x)).ToArray();
                graphicalSamples = dubbleSamples;
            } else
            {
                short[] bytes2Samples = new short[wavhdr.data_size / wavhdr.nblock_align];
                for (int M = 0; M < (wavhdr.data_size / wavhdr.nblock_align); M++)
                {
                    bytes2Samples[M] = BitConverter.ToInt16(bytesToListen, 44 + M * wavhdr.nblock_align);
                }
                double[] dubbleSamples = bytes2Samples.Select(x => (double)(x)).ToArray();
                graphicalSamples = dubbleSamples;
            }
            //this is where i pass and open a new window to display
            //Form 2 is always representing the time domain.
            Form2 newMDIChild = new Form2();
            newMDIChild.WaveHeader = wavhdr;
            newMDIChild.ByteStuff = bytesToListen;
            newMDIChild.DataWave = graphicalSamples;
            // Set the Parent Form of the Child window.
            newMDIChild.MdiParent = this;
            // Display the new form.
            newMDIChild.Show();
        }
        
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            
        }
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }
        //clicking this will pass the relevant wave header types chosen to the dll
        //and start recording audio. displays a popup to let you know if you are
        //recording or not.
        private void recordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (startRec(wavhdr.samples_per_sec, wavhdr.bits_per_sample) == 1)
            {
                recordAlert.MdiParent = this;
                // Display the new form.
                recordAlert.Show();
            }
        }
        //this is the button that stops recording and handles the two bit depths
        private void stopRecToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(stopRec() == 1)
            {
                recordAlert.Hide();
            }
            recordDataPtr.recordData dataPtr = endDialog();
            audibleBytes = new byte[dataPtr.length];
            Marshal.Copy(dataPtr.iptr, audibleBytes, 0, (int)dataPtr.length);
            double[] dubbleSamples = new double[] { 0}; 
            //handling 16 or 32 bit depth
            if (wavhdr.bits_per_sample == 32)
            {
                int[] bytes2Samples = new int[dataPtr.length / 4];
                for (int M = 0; M < (bytes2Samples.Length); M++)
                {
                    bytes2Samples[M] = BitConverter.ToInt32(audibleBytes, M * 4);
                }

                dubbleSamples = bytes2Samples.Select(x => (double)(x)).ToArray();
            } else
            {
                short[] bytes2Samples = new short[dataPtr.length / 2];
                for (int M = 0; M < (bytes2Samples.Length); M++)
                {
                    bytes2Samples[M] = BitConverter.ToInt16(audibleBytes, M * 2);
                }

                dubbleSamples = bytes2Samples.Select(x => (double)(x)).ToArray();
            }
            
            //opening the child window to display the recording.
            //Form 2 is always representing the time domain.
            Form2 newMDIChild = new Form2();
            newMDIChild.WaveHeader = wavhdr;
            newMDIChild.ByteStuff = audibleBytes;
            newMDIChild.DataWave = dubbleSamples;
            newMDIChild.RecData = dataPtr;
            // Set the Parent Form of the Child window.
            newMDIChild.MdiParent = this;
            // Display the new form.
            newMDIChild.Show();
        }
        //default recording values
        private void Form1_Load(object sender, EventArgs e)
        {
            wavhdr.samples_per_sec = 22050;
            wavhdr.bits_per_sample = 16;
            if (createDialog() == false)
            {
                Debug.WriteLine("Dialog Failed");
            }
        }
        //the toolstrip menu choices for recording sample rates and bit depth
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in bitDepthMenuTool.DropDownItems)
            {
                item.Checked = false;
            }
            wavhdr.bits_per_sample = 16;
            toolStripMenuItem2.Checked = true;
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in bitDepthMenuTool.DropDownItems)
            {
                item.Checked = false;
            }
            wavhdr.bits_per_sample = 32;
            toolStripMenuItem3.Checked = true;
        }

        private void hzToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in sampleRateToolStripMenuItem.DropDownItems)
            {
                item.Checked = false;
            }
            wavhdr.samples_per_sec = 11025;
            hzToolStripMenuItem.Checked = true;
        }

        private void hzToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            
            foreach (ToolStripMenuItem item in sampleRateToolStripMenuItem.DropDownItems)
            {
                item.Checked = false;
            }
            wavhdr.samples_per_sec = 22050;
            hzToolStripMenuItem1.Checked = true;
        }

        private void hzToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in sampleRateToolStripMenuItem.DropDownItems)
            {
                item.Checked = false;
            }
            wavhdr.samples_per_sec = 44100;
            hzToolStripMenuItem2.Checked = true;
        }
    }
}
