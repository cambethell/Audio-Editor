using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//this is the recording pointer struct that is passed to
//the win32 dll to properly access the data.
namespace projectaudio
{
    public class recordDataPtr
    {
        public struct recordData
        {
            public UInt32 length;
            public IntPtr iptr;
        };
    }
}
