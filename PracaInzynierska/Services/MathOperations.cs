using PracaInzynierska.Interfaces;
using PracaInzynierska.Models;
using System.Collections.Generic;
using System;


namespace PracaInzynierska.Services
{
    public class MathOperations: IMathOperations
    {
        //nos jako parametr
        public System.Numerics.Complex [] FFTImp(double[] tab, float time, int numberOfSamples)
        {
            float tp = time / numberOfSamples;
            float fp = 1 / tp;
            var buffer = new System.Numerics.Complex[numberOfSamples];
            
            for (int i = 0; i < numberOfSamples; i++)
            {
                System.Numerics.Complex tmp = new System.Numerics.Complex(tab[i], 0);
                buffer[i] = tmp;
            }
            MathNet.Numerics.IntegralTransforms.Fourier.Forward(buffer, MathNet.Numerics.IntegralTransforms.FourierOptions.Matlab);
     
            return buffer;
            
        }
        public  double[] absComplexToDouble(System.Numerics.Complex[]buffer,int numberOfSamples)
        {
            double[] bufferABS = new double[numberOfSamples];
            for (int i = 0; i<numberOfSamples; i++)
            {
                bufferABS[i] = buffer[i].Magnitude;
            }
   
            return bufferABS;
        }
        public double[] normalization(double[] tab, int numberOfSamples)
        {
            double sum = 0;
            for (int i = 0; i < numberOfSamples; i++)
            {
                sum += tab[i];
            }
            double diff = sum / numberOfSamples;
            double[] tabNormalized = new double[numberOfSamples];
            for (int i = 0; i < numberOfSamples; i++)
            {
                tabNormalized[i] = tab[i] - diff;
            }
            return tabNormalized;
        }
        public RecaivedData lowPassFilter( RecaivedData Data,double cutOff)
        {
            double tp = Data.time / Data.numberOfSamples;
            for (int i = 0; i < Data.numberOfSamples; i++)
            {
                if ((tp * i) > cutOff)
                {
                    Data.bar[i] = 0;
                }
               
            }
            return Data;
        }
       public RecaivedData expFilter( RecaivedData Data, double lambda)
        {

            double[] exp = new double[Data.numberOfSamples];
            double tp = Data.time / Data.numberOfSamples;
            for (int i = 0; i < Data.numberOfSamples; i++)
            {
                exp[i] = Math.Exp((-1) * lambda * (tp*i));
                Data.bar[i] = exp[i] * Data.bar[i];
            }
            return Data;
            
        }
        public void FunkcjaPrzejscia(double[] bar, double[] hammer, float freq, int numberOfSamples)//to bullshit to beda complex, czy do tego ida magnitude
        {

        }
    }
}
