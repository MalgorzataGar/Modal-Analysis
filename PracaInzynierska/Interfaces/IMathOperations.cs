using PracaInzynierska.Models;
using System.Collections.Generic;

namespace PracaInzynierska.Interfaces
{
    public interface IMathOperations
    {
        System.Numerics.Complex[] FFTImp(double[] tab, float time, int numberOfSamples);
        double[] absComplexToDouble(System.Numerics.Complex[] buffer, int numberOfSamples);
        double[] normalization(double[] tab, int numberOfSamples);
        RecaivedData lowPassFilter( RecaivedData Data, double cutOff);
        RecaivedData expFilter( RecaivedData Data, double lambda);
        double[] getRealValues(System.Numerics.Complex[] buffer, int numberOfSamples);
        double[] getImaginaryValues(System.Numerics.Complex[] buffer, int numberOfSamples);

        System.Numerics.Complex[] FRF(System.Numerics.Complex[] barfft, System.Numerics.Complex[] hammerfft, int numberOfSamples);
        System.Numerics.Complex[] Average(List<System.Numerics.Complex[]> list);
    }
}