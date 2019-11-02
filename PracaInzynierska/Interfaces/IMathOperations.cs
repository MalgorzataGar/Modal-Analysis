using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PracaInzynierska.Interfaces
{
    interface IMathOperations
    {
        double[] FFTImp(int[] tab, float time, int numberOfSamples);

       void FunkcjaPrzejscia(int[] tab, int[] tab2, float freq, int numberOfSamples);
    }
}
