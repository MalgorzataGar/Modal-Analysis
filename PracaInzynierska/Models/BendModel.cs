using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PracaInzynierska.Models
{
    public class BendModel
    {
        public BendModel(int nop)
        {
            numberOfPoints = nop;
            freaArray1 = new double[numberOfPoints];
            freaArray2 = new double[numberOfPoints];
            freaArray3 = new double[numberOfPoints];
            freaArray4 = new double[numberOfPoints];
            freaArray5 = new double[numberOfPoints];
            resonanse = new double[5];
        }

        int numberOfPoints;
        public double[] resonanse { get; set; }
        public double [] freaArray1 {get;set;}
        public double[] freaArray2 { get; set; }
        public double[] freaArray3 { get; set; }
        public  double[] freaArray4 { get; set; }
        public double[] freaArray5 { get; set; }
    }
}
