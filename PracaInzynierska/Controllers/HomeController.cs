using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using PracaInzynierska.Models;
using PracaInzynierska.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace PracaInzynierska.Controllers
{
    public class HomeController : Controller
    {
        public readonly IConnectionService ConnectionService;
        public readonly IGraph Graph;
        public readonly IMathOperations MathOperations;
      

        public HomeController(IConnectionService connectionService, IGraph graph, IMathOperations mathOperations)
        {
            ConnectionService = connectionService;
            Graph = graph;
            MathOperations = mathOperations;
            
        }
        public IActionResult Index()
        {
            int currentMeasure;
            if (HttpContext.Session.GetInt32("currentMeasure") != null)
            {
                currentMeasure = (int)HttpContext.Session.GetInt32("currentMeasure");
            }
            else
            {
                currentMeasure = -1;
                HttpContext.Session.SetInt32("currentMeasure", currentMeasure);
            }
            int currentPoint;
            if (HttpContext.Session.GetInt32("currentPoint") != null)
            {
                currentPoint = (int)HttpContext.Session.GetInt32("currentPoint");
            }
            else
            {
                currentPoint = 1;
                HttpContext.Session.SetInt32("currentPoint", currentPoint);
            }
            if (HttpContext.Session.GetInt32("frfMeasures") == null)
            {
                List<System.Numerics.Complex[]> frfMeasures = new List<System.Numerics.Complex[]>();
                HttpContext.Session.SetString("frfMeasures", JsonConvert.SerializeObject(frfMeasures));
            }
            if (HttpContext.Session.GetInt32("frfFinal") == null)
            {
                List<System.Numerics.Complex[]> frfFinal = new List<System.Numerics.Complex[]>();
                HttpContext.Session.SetString("frfFinal", JsonConvert.SerializeObject(frfFinal));
            }
            ViewBag.currentMeasure = currentMeasure;
            ViewBag.currentPoint = currentPoint;
            return View();
        }
        public IActionResult AnalizeData()
        {
            int numberOfPoints;
            if (HttpContext.Session.GetInt32("numberOfTestsPerPoint") == null)
            {
                int numberOfTestsPerPoint = Int32.Parse(HttpContext.Request.Form["numberOfTestsPerPoint"].ToString());
                HttpContext.Session.SetInt32("numberOfTestsPerPoint", numberOfTestsPerPoint); 
            }
            if (HttpContext.Session.GetInt32("numberOfPoints") == null)
            {
                numberOfPoints = Int32.Parse(HttpContext.Request.Form["numberOfPoints"].ToString());
                HttpContext.Session.SetInt32("numberOfPoints", numberOfPoints);
            }
            else numberOfPoints =(int)HttpContext.Session.GetInt32("numberOfPoints");
            // HttpContext.Session.SetInt32("numberOfPoints", numberOfPoints);
            int[] indexOfMounting = new int[2];
            if (HttpContext.Session.GetString("indexOfMounting") == null)
            {
                indexOfMounting[0] = Int32.Parse(HttpContext.Request.Form["firstMounting"].ToString());
                indexOfMounting[1] = Int32.Parse(HttpContext.Request.Form["secondMounting"].ToString());
                HttpContext.Session.SetString("indexOfMounting", JsonConvert.SerializeObject(indexOfMounting));
            }
            RecaivedData recaivedDatas = new RecaivedData();
            var currentMeasure= (int)HttpContext.Session.GetInt32("currentMeasure") ;
            currentMeasure++;
            DateTime centuryBegin = new DateTime(2019, 11, 14);
            DateTime currentDate = DateTime.Now;
            long elapsedTicks = currentDate.Ticks - centuryBegin.Ticks;
            TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);
            HttpContext.Session.SetInt32("currentMeasure", currentMeasure);
            string filename = "myFile_" + (int)elapsedSpan.TotalSeconds + ".txt";
            string str = "{'filename': '" + filename + "'}";
            JObject configJson = JObject.Parse(str);
           // ConnectionService.HandleConnection(configJson).Wait();
            recaivedDatas = ConnectionService.GetDataFromString();
            string filenameB = "bar" + DateTime.Now.ToString("HH_mm_ss");
            string filenameH = "hammer" + DateTime.Now.ToString("HH_mm_ss");
            ConnectionService.SaveToFile(filenameB, recaivedDatas.bar, recaivedDatas.time);
            ConnectionService.SaveToFile(filenameH, recaivedDatas.hammer, recaivedDatas.time);
            double[] bar_raw = recaivedDatas.bar;
            double[] hammer_raw = recaivedDatas.hammer;
            double[] bar = MathOperations.normalization(bar_raw, recaivedDatas.numberOfSamples);
            double[] hammer = MathOperations.normalization(hammer_raw, recaivedDatas.numberOfSamples);
            recaivedDatas.bar = bar;
            recaivedDatas.hammer = hammer;
            HttpContext.Session.SetString("recaivedDatas", JsonConvert.SerializeObject(recaivedDatas));
            List<DataPoint> pointsListBar = Graph.fillDataPoints(bar, recaivedDatas.numberOfSamples, (float)recaivedDatas.time);
            List<DataPoint> pointsListHammer = Graph.fillDataPoints(hammer, recaivedDatas.numberOfSamples, (float)recaivedDatas.time);
            ViewBag.TimeBar = JsonConvert.SerializeObject(pointsListBar);
            ViewBag.TimeHammer = JsonConvert.SerializeObject(pointsListHammer);
            if (HttpContext.Session.GetString("lambda") == null)
            {
                ViewBag.isLambdaSet = 0;
            }
            else
            {
                ViewBag.isLambdaSet = 1;
            }
            return View("DataTime");
        }

        public IActionResult Filtration()
        {
            var currentMeasure = (int)HttpContext.Session.GetInt32("currentMeasure");
            RecaivedData recaivedDatas = JsonConvert.DeserializeObject<RecaivedData>(HttpContext.Session.GetString("recaivedDatas"));
            double[] bar = recaivedDatas.bar;
            double[] hammer = recaivedDatas.bar;
            double lambda;
            if (HttpContext.Session.GetString("lambda") == null)
            {
                lambda = double.Parse(HttpContext.Request.Form["lambda"].ToString());
                HttpContext.Session.SetString("lambda", lambda.ToString());
            }
            else lambda = double.Parse(HttpContext.Session.GetString("lambda"));
            
            double cutOff = double.Parse(HttpContext.Request.Form["cutOff"].ToString());
            double offset = double.Parse(HttpContext.Request.Form["offset"].ToString());
            recaivedDatas= MathOperations.lowPassFilter(recaivedDatas, cutOff);
            recaivedDatas.bar = MathOperations.offset(recaivedDatas.bar, recaivedDatas.time, offset);
            recaivedDatas.hammer = MathOperations.offset(recaivedDatas.hammer, recaivedDatas.time, offset);
            recaivedDatas = MathOperations.expFilter(recaivedDatas, lambda);
            List<DataPoint> filtredBar = Graph.fillDataPoints(recaivedDatas.bar, recaivedDatas.numberOfSamples ,(float)recaivedDatas.time);
            List<DataPoint> filtredHammer = Graph.fillDataPoints(recaivedDatas.hammer,recaivedDatas.numberOfSamples , (float)recaivedDatas.time);
            ViewBag.FiltredBar = JsonConvert.SerializeObject(filtredBar);
            ViewBag.FiltredHammer = JsonConvert.SerializeObject(filtredHammer);
            System.Numerics.Complex[] bufferBar = MathOperations.FFTImp(recaivedDatas.bar,(float)recaivedDatas.time, recaivedDatas.numberOfSamples);
            System.Numerics.Complex[] bufferHammer = MathOperations.FFTImp(recaivedDatas.hammer, (float)recaivedDatas.time, recaivedDatas.numberOfSamples);
            double[] fftBar = MathOperations.absComplexToDouble(bufferBar, recaivedDatas.numberOfSamples);
            double[] fftHammer = MathOperations.absComplexToDouble(bufferHammer, recaivedDatas.numberOfSamples);
            float freq = recaivedDatas.numberOfSamples / (float)recaivedDatas.time;
            //float freq = fp * recaivedDatas.numberOfSamples;
            HttpContext.Session.SetString("freq", JsonConvert.SerializeObject(freq));
            List<DataPoint> freqPointsBar = Graph.fillDataPoints(fftBar, (recaivedDatas.numberOfSamples / 2), freq);
            ViewBag.FreqBar = JsonConvert.SerializeObject(freqPointsBar);
            List<DataPoint> freqPointsHammer = Graph.fillDataPoints(fftHammer, (recaivedDatas.numberOfSamples / 2), freq);
            ViewBag.FreqHammer = JsonConvert.SerializeObject(freqPointsHammer);
            System.Numerics.Complex[] frf = MathOperations.FRF(bufferBar, bufferHammer, recaivedDatas.numberOfSamples);
            double[] frfMagnitude = MathOperations.absComplexToDouble(frf, recaivedDatas.numberOfSamples);
            List<DataPoint> frfMagnitudePoints = Graph.fillDataPoints(frfMagnitude, recaivedDatas.numberOfSamples/2, freq);
            ViewBag.FrfMagitude = JsonConvert.SerializeObject(frfMagnitudePoints);
            double[] frfReal = MathOperations.getRealValues(frf, recaivedDatas.numberOfSamples);
            List<DataPoint> frfRealPoints = Graph.fillDataPoints(frfReal, recaivedDatas.numberOfSamples/2, freq);
            ViewBag.FrfReal = JsonConvert.SerializeObject(frfRealPoints);
            double[] frfImaginary = MathOperations.getImaginaryValues(frf, recaivedDatas.numberOfSamples);
            List<DataPoint> frfImaginaryPoints = Graph.fillDataPoints(frfImaginary, recaivedDatas.numberOfSamples/2, freq);
            ViewBag.FrfImaginary = JsonConvert.SerializeObject(frfImaginaryPoints);
            HttpContext.Session.SetString("frfCurrent", JsonConvert.SerializeObject(frf));

            return View("Filtred");
        }
        public IActionResult Accept()
        {
            var currentMeasure = (int)HttpContext.Session.GetInt32("currentMeasure");
            var currentPoint = (int)HttpContext.Session.GetInt32("currentPoint");
            int acceptance = Int32.Parse(HttpContext.Request.Form["acceptance"].ToString());

            if (acceptance == 1)
            {
                var numberOfTestsPerPoint = HttpContext.Session.GetInt32("numberOfTestsPerPoint");
                var numberOfPoints = HttpContext.Session.GetInt32("numberOfPoints");
                List<System.Numerics.Complex[]> frfMeasures = JsonConvert.DeserializeObject<List<System.Numerics.Complex[]>>(HttpContext.Session.GetString("frfMeasures"));
                List<System.Numerics.Complex[]> frfFinal = JsonConvert.DeserializeObject<List<System.Numerics.Complex[]>>(HttpContext.Session.GetString("frfFinal"));
                System.Numerics.Complex[] frfCurrent = JsonConvert.DeserializeObject<System.Numerics.Complex[]>(HttpContext.Session.GetString("frfCurrent"));
                frfMeasures.Add(frfCurrent);
                HttpContext.Session.SetString("frfMeasures", JsonConvert.SerializeObject(frfMeasures));
                if (currentMeasure == numberOfTestsPerPoint-1)
                {
                    System.Numerics.Complex[] frfAverage = MathOperations.Average(frfMeasures);
                    frfAverage = MathOperations.movingAverage(frfAverage, 7);
                    string filename = "frf" + currentPoint;
                    float freq = JsonConvert.DeserializeObject<float>(HttpContext.Session.GetString("freq"));
                    ConnectionService.SaveToFile(filename, frfAverage, freq);
                    frfFinal.Add(frfAverage);
                    frfMeasures.Clear();
                    currentMeasure = -1;
                    HttpContext.Session.SetString("frfFinal", JsonConvert.SerializeObject(frfFinal));
                    HttpContext.Session.SetString("frfMeasures", JsonConvert.SerializeObject(frfMeasures));
                    if (currentPoint != numberOfPoints)
                    {
                        currentPoint++;
                    }
                    else
                    {
                        if (numberOfPoints == 10)
                        { 
                            return RedirectToAction("Comparison");
                        }
                        return RedirectToAction("Final");
                    }
                }

            }
            else if(acceptance == 0)
            {
                currentMeasure--;
            }

            HttpContext.Session.SetInt32("currentMeasure", currentMeasure);
            HttpContext.Session.SetInt32("currentPoint", currentPoint);
            return RedirectToAction("Index");
        }
        public IActionResult Comparison()
        {
            var numberOfPoints = HttpContext.Session.GetInt32("numberOfPoints");
            double[] frf1, frf2, frf3, frf4, frf5, frf6, frf7, frf8, frf9, frf10;
            List<DataPoint> frfMagnitudePoints = new List<DataPoint>();
            float freq = JsonConvert.DeserializeObject<float>(HttpContext.Session.GetString("freq"));
            List<System.Numerics.Complex[]> frfFinal = JsonConvert.DeserializeObject<List<System.Numerics.Complex[]>>(HttpContext.Session.GetString("frfFinal"));
             frf1 = MathOperations.absComplexToDouble(frfFinal[0], frfFinal[0].Length);
            frfMagnitudePoints = Graph.fillDataPoints(frf1, (frfFinal[0].Length) / 2, freq);
            ViewBag.frf1 = JsonConvert.SerializeObject(frfMagnitudePoints);
            frf2 = MathOperations.absComplexToDouble(frfFinal[1], frfFinal[0].Length);
            frfMagnitudePoints = Graph.fillDataPoints(frf2, (frfFinal[0].Length) / 2, freq);
            ViewBag.frf2 = JsonConvert.SerializeObject(frfMagnitudePoints);
            frf3 = MathOperations.absComplexToDouble(frfFinal[2], frfFinal[0].Length);
            frfMagnitudePoints = Graph.fillDataPoints(frf3, (frfFinal[0].Length) / 2, freq);
            ViewBag.frf3 = JsonConvert.SerializeObject(frfMagnitudePoints);
            frf4 = MathOperations.absComplexToDouble(frfFinal[3], frfFinal[0].Length);
            frfMagnitudePoints = Graph.fillDataPoints(frf4, (frfFinal[0].Length) / 2, freq);
            ViewBag.frf4 = JsonConvert.SerializeObject(frfMagnitudePoints);
            frf5 = MathOperations.absComplexToDouble(frfFinal[4], frfFinal[0].Length);
            frfMagnitudePoints = Graph.fillDataPoints(frf5, (frfFinal[0].Length) / 2, freq);
            ViewBag.frf5 = JsonConvert.SerializeObject(frfMagnitudePoints);
            frf6 = MathOperations.absComplexToDouble(frfFinal[5], frfFinal[0].Length);
            frfMagnitudePoints = Graph.fillDataPoints(frf6, (frfFinal[0].Length) / 2, freq);
            ViewBag.frf6 = JsonConvert.SerializeObject(frfMagnitudePoints);
            frf7 = MathOperations.absComplexToDouble(frfFinal[6], frfFinal[0].Length);
            frfMagnitudePoints = Graph.fillDataPoints(frf7, (frfFinal[0].Length) / 2, freq);
            ViewBag.frf7 = JsonConvert.SerializeObject(frfMagnitudePoints);
            frf8 = MathOperations.absComplexToDouble(frfFinal[7], frfFinal[0].Length);
            frfMagnitudePoints = Graph.fillDataPoints(frf8, (frfFinal[0].Length) / 2, freq);
            ViewBag.frf8 = JsonConvert.SerializeObject(frfMagnitudePoints);
            frf9 = MathOperations.absComplexToDouble(frfFinal[8], frfFinal[0].Length);
            frfMagnitudePoints = Graph.fillDataPoints(frf9, (frfFinal[0].Length) / 2, freq);
            ViewBag.frf9 = JsonConvert.SerializeObject(frfMagnitudePoints);
            frf10 = MathOperations.absComplexToDouble(frfFinal[9], frfFinal[0].Length);
            frfMagnitudePoints = Graph.fillDataPoints(frf10, (frfFinal[0].Length) / 2, freq);
            ViewBag.frf10 = JsonConvert.SerializeObject(frfMagnitudePoints);

            return View();
        }
        public IActionResult Final()
        {
            var numberOfPoints = HttpContext.Session.GetInt32("numberOfPoints");
            double[] frfMagnitude;
            List<DataPoint> frfMagnitudePoints = new List<DataPoint>();
            float freq = JsonConvert.DeserializeObject<float>(HttpContext.Session.GetString("freq"));
            List<System.Numerics.Complex[]> frfFinal = JsonConvert.DeserializeObject<List<System.Numerics.Complex[]>>(HttpContext.Session.GetString("frfFinal"));
            System.Numerics.Complex[] frfAverage = MathOperations.Average(frfFinal);//obliczaie ostatecznej frf
            ConnectionService.SaveToFile("final", frfAverage, freq);
            frfMagnitude = MathOperations.absComplexToDouble(frfAverage, frfAverage.Length);
            frfMagnitudePoints  = Graph.fillDataPoints(frfMagnitude, (frfAverage.Length) / 2, freq);
            ViewBag.frf = JsonConvert.SerializeObject(frfMagnitudePoints);
            return View();
        }
        public IActionResult Resonance()
        {
            int numberOfPoints = (int)HttpContext.Session.GetInt32("numberOfPoints");
            BendModel bend = new BendModel(numberOfPoints);
            float freq = JsonConvert.DeserializeObject<float>(HttpContext.Session.GetString("freq"));
            List<System.Numerics.Complex[]> frfFinal = JsonConvert.DeserializeObject<List<System.Numerics.Complex[]>>(HttpContext.Session.GetString("frfFinal"));
            bool d;
            double [] freqtable= new double [5];
            string name;
            double temp;
            for (int i = 0; i < 5; i++)
            {   name = "freq" + (i+1);
                d = double.TryParse(HttpContext.Request.Form[name].ToString(), out temp );
                if(d)
                {
                    freqtable[i] = temp;
                }
                else
                {
                    freqtable[i] = 0;
                }
                
            }
            int[] indexOfMounting = JsonConvert.DeserializeObject<int[]>((HttpContext.Session.GetString("indexOfMounting").ToString()));
            List<DataPoint> freq1 = new List<DataPoint>();
            List<DataPoint> freq2 = new List<DataPoint>();
            List<DataPoint> freq3 = new List<DataPoint>();
            List<DataPoint> freq4 = new List<DataPoint>();
            List<DataPoint> freq5 = new List<DataPoint>();
            MathOperations.bendingArrays(ref bend, freqtable, frfFinal, freq);
            bend.resonance = freqtable;
            freq1 = Graph.fillBendingPoints(bend.freaArray1, indexOfMounting);
            ViewBag.freq1 = JsonConvert.SerializeObject(freq1);
            freq2 = Graph.fillBendingPoints(bend.freaArray2, indexOfMounting);
            ViewBag.freq2 = JsonConvert.SerializeObject(freq2);
            freq3 = Graph.fillBendingPoints(bend.freaArray3, indexOfMounting);
            ViewBag.freq3 = JsonConvert.SerializeObject(freq3);
            freq4 = Graph.fillBendingPoints(bend.freaArray4, indexOfMounting);
            ViewBag.freq4 = JsonConvert.SerializeObject(freq4);
            freq5 = Graph.fillBendingPoints(bend.freaArray5, indexOfMounting);
            ViewBag.freq5 = JsonConvert.SerializeObject(freq5);

            return View(bend);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
       
    }
}
