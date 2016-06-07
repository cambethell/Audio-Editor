using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projectaudio
{
    public class WindowingClass
    { 

        public static double[] TriangleWindow(double[] windowWave)
        {
            for (int i = 0; i < windowWave.Length; i++)
            {
                double winFactor = 1 - Math.Abs((i - ((windowWave.Length - 1) / 2)) / (windowWave.Length / 2));
                windowWave[i] = windowWave[i] * winFactor;
            }
            return windowWave;
        }

        public static double[] BlackmanHarrisWindow(double[] windowWave)
        {
            double a0 = 0.357875;
            double a1 = 0.48829;
            double a2 = 0.14128;
            double a3 = 0.01168;

            for (int i = 0; i < windowWave.Length; i++)
            {
                double winFactor = a0 
                    - a1 * Math.Cos((2 * Math.PI * i) / (windowWave.Length - 1)) 
                    + a2 * Math.Cos((4 * Math.PI * i) / (windowWave.Length - 1)) 
                    - a3 * Math.Cos((6 * Math.PI * i) / (windowWave.Length - 1));
                windowWave[i] = windowWave[i] * winFactor;
            }
            return windowWave;
        }

        public static double[] HammingWindow(double[] theData)
        {
            
            double alpha = 25 / 46;
            double beta = 1 - alpha;
            for (int i = 0; i < theData.Length; i++)
            {
                double winFactor = alpha - beta * Math.Cos( (2 * Math.PI * i) / (theData.Length - 1));
                theData[i] = theData[i] * winFactor;
            }
            return theData;
        }

    }
}
