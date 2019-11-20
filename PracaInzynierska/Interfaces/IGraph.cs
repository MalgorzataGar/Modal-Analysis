using System.Collections.Generic;
using PracaInzynierska.Models;

namespace PracaInzynierska.Interfaces
{
    public interface IGraph
    {
        List<DataPoint> fillDataPoints(double []tab,int numberOfSamples,float time);
        List<DataPoint> fillBendingPoints(double[] freq, int[] indexsOf);
    }
}
