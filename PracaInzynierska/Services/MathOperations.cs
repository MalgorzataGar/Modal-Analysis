using PracaInzynierska.Interfaces;


namespace PracaInzynierska.Services
{
    public class MathOperations: IMathOperations
    {
        //nos jako parametr
        public System.Numerics.Complex [] FFTImp(double[] tab, float time, int numberOfSamples)
        {
            float tp = time / numberOfSamples;
            float fp = 1 / tp;
            double sum=0;
            var buffer = new System.Numerics.Complex[numberOfSamples];
            for (int i = 0; i < numberOfSamples; i++)
            {
                sum += tab[i];
            }
            double diff = sum / numberOfSamples;
            double[] tabNormalized = new double [numberOfSamples];
            for (int i = 0; i < numberOfSamples; i++)
            {
                tabNormalized[i] = tab[i] - diff;
            }
            for (int i = 0; i < numberOfSamples; i++)
            {
                System.Numerics.Complex tmp = new System.Numerics.Complex(tabNormalized[i], 0);
                buffer[i] = tmp;
            }
            MathNet.Numerics.IntegralTransforms.Fourier.Forward(buffer, MathNet.Numerics.IntegralTransforms.FourierOptions.Matlab);
     
            return buffer;
            
        }
        public  double[] absComplexToDouble(System.Numerics.Complex[]buffer,int numberOfSamples)
        {
            double[] bufferABS = new double[numberOfSamples];
            for (int i = 0; i<numberOfSamples; i++)
            {
                bufferABS[i] = buffer[i].Magnitude;
            }
   
            return bufferABS;
        }
        
        public void FunkcjaPrzejscia(double[] bar, double[] hammer, float freq, int numberOfSamples)//to bullshit to beda complex, czy do tego ida magnitude
        {

        }
    }
}
