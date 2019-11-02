using System.Collections.Generic;
using PracaInzynierska.Models;

namespace PracaInzynierska.Interfaces
{
    public interface IGraph
    {
        double[] dataFromFile();
        List<DataPoint> fillDataPoints(double []tab,int numberOfSamples,float time);
    }
}
