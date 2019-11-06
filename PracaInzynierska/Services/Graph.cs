using System;
using System.Collections.Generic;
using System.IO;
using PracaInzynierska.Models;
using PracaInzynierska.Interfaces;

namespace PracaInzynierska.Services
{
    public class Graph: IGraph
    {
       public void dataFromFile(out double[]bar, out double [] hammer)
        {
            List<double> list1 = new List<double>();
            List<double> list2 = new List<double>();
            using (var reader = new StreamReader(@"C:\Users\USER\Desktop\dane.csv"))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    string[] values = line.Split(';');
                    double value1 = Double.Parse(values[0]);
                    double value2 = Double.Parse(values[1]);
                    list1.Add(value1);
                    list2.Add(value2);
                }
            }
            bar = list1.ToArray();
            hammer = list2.ToArray();
            
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
