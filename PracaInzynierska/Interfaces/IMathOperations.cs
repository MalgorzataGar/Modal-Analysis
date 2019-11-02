

namespace PracaInzynierska.Interfaces
{
    interface IMathOperations
    {
       System.Numerics.Complex[] FFTImp(double[] tab, float time, int numberOfSamples);
        double[] absComplexToDouble(System.Numerics.Complex[] buffer, int numberOfSamples);

       void FunkcjaPrzejscia(double[] bar, double[] hammer, float freq, int numberOfSamples);//czy freq i nos potzrebne count()
    }
}
