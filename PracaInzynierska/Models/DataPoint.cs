using System;
using System.Runtime.Serialization;


namespace PracaInzynierska.Models
{
    [DataContract]
    public class DataPoint
    {
        public DataPoint(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
        public double GetY()
        {
            return (double)Y;
        }
        public double GetX()
        {
            return (double)X;
        }
       
        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "x")]
        public Nullable<double> X = null;

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "y")]
        public Nullable<double> Y = null;
    }
}  

