using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

//this is the class that defines the transformation functions
//the threaded DFT, unthreaded DFT, and the inverse DFT
namespace projectaudio
{
    public class transform
    {
        //this is the DFT process that gets split into two threads.
        //it must be stressed that this DFT is only representative
        //of the frequency domain, since it does not divide by N
        static void lilThreadDFT(double[] data, double[] dft, int start, int end)
        {
            double freqBinfRe = 0;
            double freqBinfIm = 0;
            for (int f = start; f < end; f++)
            {
                freqBinfRe = 0;
                freqBinfIm = 0;
                for (int t = 0; t < data.Length; t++)
                {
                    freqBinfRe += data[t] * (Math.Cos(2 * Math.PI * f * t / data.Length));
                    freqBinfIm -= data[t] * (Math.Sin(2 * Math.PI * f * t / data.Length));
                }
                dft[f] = Math.Sqrt(freqBinfRe * freqBinfRe + freqBinfIm * freqBinfIm);
            }
        }
        //this is the function that splits the dft process into two
        //lilthreadDFT thread processes on two halves of the data.
        public static double[] threadDFT(double[] data)
        {
            double[] dataDFT = new double[data.Length];
            Thread t1 = new Thread(() => lilThreadDFT(data, dataDFT, 0, dataDFT.Length / 2));
            t1.Start();
            Thread t2 = new Thread(() => lilThreadDFT(data, dataDFT, (dataDFT.Length / 2) + 1, dataDFT.Length));
            t2.Start();
            t1.Join();
            t2.Join();
            return dataDFT;
        }
        //this is the original DFT function. it only exists now
        //as proof of concept and is not used in regular practice.
        public static double[] DFT(double[] data)
        {
            double[] dataDFT = new double[data.Length];
            double freqBinfRe = 0;
            double freqBinfIm = 0;
            //THIS DO THE DFT
            for (int f = 0; f < dataDFT.Length; f++)
            {
                freqBinfRe = 0;
                freqBinfIm = 0;
                for (int t = 0; t < data.Length; t++)
                {

                    freqBinfRe += data[t] * (Math.Cos(2 * Math.PI * f * t / data.Length));
                    freqBinfIm -= data[t] * (Math.Sin(2 * Math.PI * f * t / data.Length));
                }

                dataDFT[f] = Math.Sqrt(freqBinfRe * freqBinfRe + freqBinfIm * freqBinfIm);
            }

            return dataDFT;
        }
        //this is the inverse DFT function.
        public static double[] inverseDFT(complex[] values)
        {
            double[] dataSample = new double[values.Length];
            double sampleRe = 0;
            double sampleIm = 0;
            for (int t = 0; t < dataSample.Length; t++)
            {
                sampleRe = 0;
                sampleIm = 0;
                for (int f = 0; f < values.Length; f++)
                {
                    sampleRe += values[f].real * (Math.Cos(2 * Math.PI * f * t / values.Length));
                    sampleIm += values[f].imag * (Math.Sin(2 * Math.PI * f * t / values.Length));
                }
                //the datasample[t] value is heavily simplified since we 
                //only need the real component of this result to
                //display the data again in the time domain.
                //the dividing by N is done here.
                dataSample[t] = (sampleRe - sampleIm) / values.Length;
            }
            return dataSample;
        }
    }
}
