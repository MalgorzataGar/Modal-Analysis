using PracaInzynierska.Interfaces;
using PracaInzynierska.Models;
using System.Collections.Generic;
using System;


namespace PracaInzynierska.Services
{
    public class MathOperations : IMathOperations
    {
        
        public System.Numerics.Complex[] FFTImp(double[] tab, float time, int numberOfSamples)
        {
            
            var buffer = new System.Numerics.Complex[numberOfSamples];

            for (int i = 0; i < numberOfSamples; i++)
            {
                System.Numerics.Complex tmp = new System.Numerics.Complex(tab[i], 0);
                buffer[i] = tmp;
            }
            MathNet.Numerics.IntegralTransforms.Fourier.Forward(buffer, MathNet.Numerics.IntegralTransforms.FourierOptions.Matlab);

            return buffer;

        }
        public double[] absComplexToDouble(System.Numerics.Complex[] buffer, int numberOfSamples)
        {
            double[] bufferABS = new double[numberOfSamples];
            for (int i = 0; i < numberOfSamples; i++)
            {
                bufferABS[i] = buffer[i].Magnitude;
            }

            return bufferABS;
        }
        public double[] getRealValues(System.Numerics.Complex[] buffer, int numberOfSamples)
        {
            double[] realValues = new double[numberOfSamples];
            for (int i = 0; i < numberOfSamples; i++)
            {
                realValues[i] = buffer[i].Real;
            }

            return realValues;
        }
        public double[] getImaginaryValues(System.Numerics.Complex[] buffer, int numberOfSamples)
        {
            double[] imaginaryValues = new double[numberOfSamples];
            for (int i = 0; i < numberOfSamples; i++)
            {
                imaginaryValues[i] = buffer[i].Imaginary;
            }

            return imaginaryValues;
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
        public RecaivedData lowPassFilter(RecaivedData Data, double cutOff)
        {
            double tp = Data.time / Data.numberOfSamples;
            for (int i = 0; i < Data.numberOfSamples; i++)
            {
                if ((tp * i) > cutOff)
                {
                    Data.hammer[i] = 0;
                }

            }
            return Data;
        }
        public RecaivedData expFilter(RecaivedData Data, double lambda)
        {

            double[] exp = new double[Data.numberOfSamples];
            double tp = Data.time / Data.numberOfSamples;
            for (int i = 0; i < Data.numberOfSamples; i++)
            {
                exp[i] = Math.Exp((-1) * lambda * (tp * i));
                Data.bar[i] = exp[i] * Data.bar[i];
            }
            return Data;

        }
        public System.Numerics.Complex[] FRF(System.Numerics.Complex[] barfft, System.Numerics.Complex[] hammerfft, int numberOfSamples)
        {
            System.Numerics.Complex[] frf = new System.Numerics.Complex[numberOfSamples];
            for (int i = 0; i < numberOfSamples; i++)
            {
                frf[i] = System.Numerics.Complex.Divide(barfft[i], hammerfft[i]);
            }
            return frf;
        }
        public System.Numerics.Complex[] Average(List<System.Numerics.Complex[]> list)
        {
            System.Numerics.Complex[] average = new System.Numerics.Complex[list[0].Length];
            for (int i = 0; i < list[0].Length; i++)
            {
                double real = 0;
                double imaginary = 0;
                foreach (var element in list)
                {
                    real += element[i].Real;
                    imaginary += element[i].Imaginary;
                }
                real = real / list.Count;
                imaginary = imaginary / list.Count;
                average[i] = new System.Numerics.Complex(real, imaginary);
            }
            return average;
        }
        public void bendingArrays(ref BendModel bend, double[] freqtable, List<System.Numerics.Complex[]> frfFinal, double freqTotat)
        {
            double freq = freqTotat / frfFinal[0].Length;
            for (int i = 0; i < freqtable.Length; i++)//petla po czestotliwoscisch
            {
                double currentFreq = freqtable[i];

                int min = 0; 
                double temp = 0;
                for (int j = 0; j < frfFinal[0].Length; j++)//wybor min czest
                {
                    if (temp == 0)
                    {
                        temp = System.Math.Abs(j * freq - currentFreq);
                    }
                    else
                    {
                        if (temp > System.Math.Abs(j * freq - currentFreq))
                        {
                            temp = System.Math.Abs(j * freq - currentFreq);
                        }
                        else
                        {
                            min = j;
                            break;
                        }
                    }

                }
                switch (i + 1)
                {
                    case 1:
                        for (int z = 0; z < bend.freaArray1.Length; z++)
                        {
                            bend.freaArray1[z] = (frfFinal[z])[min].Imaginary;
                        }
                        break;
                    case 2:
                        for (int z = 0; z < bend.freaArray1.Length; z++)
                        {
                            bend.freaArray2[z] = (frfFinal[z])[min].Imaginary;
                        }
                        break;
                    case 3:
                        for (int z = 0; z < bend.freaArray1.Length; z++)
                        {
                            bend.freaArray3[z] = (frfFinal[z])[min].Imaginary;
                        }
                        break;
                    case 4:
                        for (int z = 0; z < bend.freaArray1.Length; z++)
                        {
                            bend.freaArray4[z] = (frfFinal[z])[min].Imaginary;
                        }
                        break;
                    case 5:
                        for (int z = 0; z < bend.freaArray1.Length; z++)
                        {
                            bend.freaArray5[z] = (frfFinal[z])[min].Imaginary;
                        }
                        break;
                }
            }
            return;
        }
        public System.Numerics.Complex[] movingAverage(System.Numerics.Complex[] table, int windowSize)
        {
            int window = (windowSize - 1) / 2;
            System.Numerics.Complex[] output = new System.Numerics.Complex[table.Length];
          
            for (int i = 0; i < table.Length; i++)
            {
                if (i < window || table.Length - i <= window)
                {
                    output[i] = table[i];
                }
                else
                {
                    double real = 0;
                    double imaginary = 0;
                    for (int j = -window; j <= window; j++)
                    {
                        real += table[i + j].Real;
                        imaginary+= table[i + j].Imaginary;
                    }
                    real = real / table.Length;
                    imaginary = imaginary / table.Length;
                    output[i] = new System.Numerics.Complex(real, imaginary);
                }
            }
            return output;
        }
        public double[] offset(double[] table, double time, double offset )
        {
            double temp = 0;
            double tp = time / table.Length;
            int min=0;
            double[] output = new double[table.Length];
            for (int j = 0; j < table.Length; j++)//wybor min czest
            {
                if (temp == 0)
                {
                    temp = Math.Abs(j * tp - offset);
                }
                else
                {
                    if (temp > Math.Abs(j * tp - offset))
                    {
                        temp = Math.Abs(j * tp - offset);
                    }
                    else
                    {
                        min = j;
                        break;
                    }
                }

            }
            for (int i = 0; i < table.Length; i++)
            {
                if (i + min < table.Length)
                {
                    output[i] = table[i + min];
                }
                else
                {
                    output[i] = 0;
                }
            }
            return output;
        }
    }
}



