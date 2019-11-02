using System;
using System.Collections.Generic;
using System.IO;
using PracaInzynierska.Models;
using PracaInzynierska.Interfaces;

namespace PracaInzynierska.Services
{
    public class Graph: IGraph
    {
       public double[] dataFromFile()
        {
            List<double> list = new List<double>();
            using (var reader = new StreamReader(@"C:\Users\USER\Desktop\dane.csv"))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    double value = Double.Parse(line);
                    list.Add(value);
                }
            }
            double[] tab = list.ToArray();
            return tab;
        }
       public List<DataPoint> fillDataPoints(double[]tab,int numberOfSamples,float time)
        {
            List<DataPoint> dataPoints = new List<DataPoint>();
            float tp = time / numberOfSamples;

            for (int i = 0; i < numberOfSamples; i++)
            {

                dataPoints.Add(new DataPoint(tp * i, tab[i]));//czy nie od tp+tp*i
            }
            return dataPoints;
        }

    }

}
