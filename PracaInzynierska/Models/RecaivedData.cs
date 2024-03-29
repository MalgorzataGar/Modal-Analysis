﻿using System;
using System.Collections.Generic;


namespace PracaInzynierska.Models
{
    public class RecaivedData
    {
        public double[] bar { get; set; }
        public double[] hammer { get; set; }
        public double time { get; set; }
        public int numberOfSamples { get; set; }
        public RecaivedData()
        {
            numberOfSamples = 1500;
            bar = new double[numberOfSamples];
            hammer = new double[numberOfSamples];
        }
    }
}
