using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PracaInzynierska.Interfaces;
using MathNet;

namespace PracaInzynierska.Services
{
    public class MathOperations: IMathOperations
    {
        public double [] FFTImp(int[] tab, float time, int numberOfSamples)
        {
            float tp = time / numberOfSamples;
            float fp = 1 / tp;
            int sum=0;
            var buffer = new System.Numerics.Complex[numberOfSamples];
            for (int i = 0; i < numberOfSamples; i++)
            {
                sum += tab[i];
            }
            float diff = sum / numberOfSamples;
            float[] tabNormalized = new float [numberOfSamples];
            for (int i = 0; i < numberOfSamples; i++)
            {
                tabNormalized[i] = tab[i] - diff;
            }
            for (int i = 0; i < numberOfSamples; i++)
            {
                System.Numerics.Complex tmp = new System.Numerics.Complex(tabNormalized[i], 0);
                buffer[i] = tmp;
            }
            MathNet.Numerics.IntegralTransforms.Fourier.Forward(buffer, MathNet.Numerics.IntegralTransforms.FourierOptions.Matlab);
            //do wykresy abs fft
            double[] bufferABS = new double [numberOfSamples];
            for (int i = 0; i < numberOfSamples; i++)
            {
                bufferABS[i] = buffer[i].Magnitude;
            }
            float df = fp / numberOfSamples;
            return bufferABS;
           // float tablica_czest = 0:df: fp - df;
        }
        public void FunkcjaPrzejscia(int[] tab, int[] tab2, float freq, int numberOfSamples)//to bullshit to beda complex
        {
        }
    }
}
