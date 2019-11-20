using System;
using System.Collections.Generic;
using System.IO;
using PracaInzynierska.Models;
using PracaInzynierska.Interfaces;

namespace PracaInzynierska.Services
{
    public class Graph : IGraph
    {

        public List<DataPoint> fillDataPoints(double[] tab, int numberOfSamples, float time)
        {
            List<DataPoint> dataPoints = new List<DataPoint>();
            float tp = time / numberOfSamples;

            for (int i = 0; i < numberOfSamples; i++)
            {

                dataPoints.Add(new DataPoint(tp * i, tab[i]));//czy nie od tp+tp*i
            }
            return dataPoints;
        }
        public List<DataPoint> fillBendingPoints(double [] freq, int [] indexsOf)
        {
            int numberOfPoints = freq.Length;
            List<DataPoint> dataPoints = new List<DataPoint>();
            if (indexsOf[1] != -1)
            {
                //mocowanie poczatek koniec
                dataPoints.Add(new DataPoint(1, 0));
                for (int i = 0; i < numberOfPoints; i++)
                {
                    dataPoints.Add(new DataPoint(i + 2, freq[i]));
                }
                dataPoints.Add(new DataPoint(numberOfPoints + 2, 0));
            }
            else if (indexsOf[0] == 0)
            {
                //mocowanie na poczatku
                dataPoints.Add(new DataPoint(1, 0));
                for (int i = 0; i < numberOfPoints; i++)
                {
                    dataPoints.Add(new DataPoint(i + 2, freq[i]));
                }
            }
            else
            {//mocoanie na koncu
                for (int i = 0; i < numberOfPoints; i++)
                {
                    dataPoints.Add(new DataPoint(i+1,freq[i]));
                }
                dataPoints.Add(new DataPoint(numberOfPoints + 1, 0));
            }
            
            return dataPoints;

        }
    }
}
