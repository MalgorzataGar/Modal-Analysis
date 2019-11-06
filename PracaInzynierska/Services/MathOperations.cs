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
        public List<DataPoint> lowPassFilter(List<DataPoint> dataPoints, double cutOffFrequency,int numberOfSamples)
        {
            List<DataPoint> filtred = new List<DataPoint>();
            for (int i = 0; i < numberOfSamples; i++)
            {
                if (dataPoints[i].GetX() < cutOffFrequency)
                {
                    filtred.Add(new DataPoint(dataPoints[i].GetX(), dataPoints[i].GetY()));
                }
                else
                {
                    filtred.Add(new DataPoint(dataPoints[i].GetX(), 0));
                }
            }
            return filtred;
        }
       public List<DataPoint> expFilter(List<DataPoint> dataPoints, double lambda, int numberOfSamples)
        {
            List<DataPoint> filtred = new List<DataPoint>();
            double[] exp = new double[numberOfSamples];
            for (int i = 0; i < numberOfSamples; i++)
            {
                exp[i] = Math.Exp((-1) * lambda * dataPoints[i].GetX());
                filtred.Add(new DataPoint(dataPoints[i].GetX(), exp[i]*dataPoints[i].GetY()));
            }
            return filtred;
        }
        public void FunkcjaPrzejscia(double[] bar, double[] hammer, float freq, int numberOfSamples)//to bullshit to beda complex, czy do tego ida magnitude
        {

        }
    }
}
