using System.Collections.Generic;
using PracaInzynierska.Models;

namespace PracaInzynierska.Interfaces
{
    public interface IGraph
    {
        void dataFromFile(out double[] bar, out double[]hammer);
        List<DataPoint> fillDataPoints(double []tab,int numberOfSamples,float time);
    }
}
