using PracaInzynierska.Models;
using System.Collections.Generic;

namespace PracaInzynierska.Interfaces
{
    public interface IMathOperations
    {
        System.Numerics.Complex[] FFTImp(double[] tab, float time, int numberOfSamples);
        double[] absComplexToDouble(System.Numerics.Complex[] buffer, int numberOfSamples);
        double[] normalization(double[] tab, int numberOfSamples);
        List<DataPoint> lowPassFilter(List<DataPoint> dataPoints, double cutOffFrequency, int numberOfSamples);
        List<DataPoint> expFilter(List<DataPoint> dataPoints, double lambda, int numberOfSamples);

       void FunkcjaPrzejscia(double[] bar, double[] hammer, float freq, int numberOfSamples);//czy freq i nos potzrebne count()
    }
}
