using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//this is the complex number class i designed.
//it is not exactly necessary since it is only utilized
//in the filtering section where assigning 1's and 0's could
//be done without a struct to define the complex number.
//this serves as a struct that could be used elsewhere, if the
//need for complex numbers was there.
namespace projectaudio
{
    public class complex
    {

    public double real;
    public double imag; 

    public complex()
    {
        real = 0;
        imag = 0;
    }

    public complex(double r, double i)
    {
        real = r;
        imag = i;
    }

    public double Magnitude()
    {
        return Math.Sqrt(Math.Pow(real, 2) * Math.Pow(imag, 2));
    }

    }
}
