using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projectaudio
{
    //this class defines the wave header as normal
    public class waveHead
    {
        public struct _wave_file_hdr_
        {
            public int RIFF;
            public int filesize_minus_4;
            public int WAVE;
            public int fmt_;
            public int fmt_size;
            public short format_tag;
            public short nchannels;
            public int samples_per_sec;
            public int avg_bytes_per_sec;
            public short nblock_align;
            public short bits_per_sample;
            public int data;
            public uint data_size;
        };
    }
}
