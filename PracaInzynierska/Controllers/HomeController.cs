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
            if (HttpContext.Session.GetInt32("numberOfTestsPerPoint") == null)
            {
                int numberOfTestsPerPoint = Int32.Parse(HttpContext.Request.Form["numberOfTestsPerPoint"].ToString());
                HttpContext.Session.SetInt32("numberOfTestsPerPoint", numberOfTestsPerPoint);
               // int numberOfPoints = Int32.Parse(HttpContext.Request.Form["numberOfPoints"].ToString());
                //HttpContext.Session.SetInt32("numberOfPoints", numberOfPoints);
            }
            int numberOfPoints = 2;
            HttpContext.Session.SetInt32("numberOfPoints", numberOfPoints);
            RecaivedData recaivedDatas = new RecaivedData();//czy nie na list
            var currentMeasure= (int)HttpContext.Session.GetInt32("currentMeasure") ;
            currentMeasure++;
            DateTime centuryBegin = new DateTime(2019, 11, 14);
            DateTime currentDate = DateTime.Now;
            long elapsedTicks = currentDate.Ticks - centuryBegin.Ticks;
            TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);
            HttpContext.Session.SetInt32("currentMeasure", currentMeasure);
            string filename = "myFile_" + (int)elapsedSpan.TotalSeconds + ".txt";//time and date? zmien
            string str = "{'filename': '" + filename + "'}";
            JObject configJson = JObject.Parse(str);
           // ConnectionService.HandleConnection(configJson).Wait();
            //recaivedDatas = ConnectionService.GetDataFromString();
            double[] bar_raw = recaivedDatas.bar;
            double[] hammer_raw = recaivedDatas.hammer;
            Graph.dataFromFile(out bar_raw, out hammer_raw);
            double[] bar = MathOperations.normalization(bar_raw, recaivedDatas.numberOfSamples);
            double[] hammer = MathOperations.normalization(hammer_raw, recaivedDatas.numberOfSamples);
            recaivedDatas.bar = bar;
            recaivedDatas.hammer = hammer;
            HttpContext.Session.SetString("recaivedDatas", JsonConvert.SerializeObject(recaivedDatas));
            List<DataPoint> pointsListBar = Graph.fillDataPoints(bar, recaivedDatas.numberOfSamples, (float)recaivedDatas.time);
            List<DataPoint> pointsListHammer = Graph.fillDataPoints(hammer, recaivedDatas.numberOfSamples, (float)recaivedDatas.time);
            ViewBag.TimeBar = JsonConvert.SerializeObject(pointsListBar);
            ViewBag.TimeHammer = JsonConvert.SerializeObject(pointsListHammer);
            return View("DataTime");
        }

        public IActionResult Filtration()
        {
            var currentMeasure = (int)HttpContext.Session.GetInt32("currentMeasure");
            RecaivedData recaivedDatas = JsonConvert.DeserializeObject<RecaivedData>(HttpContext.Session.GetString("recaivedDatas"));
            double[] bar = recaivedDatas.bar;
            double[] hammer = recaivedDatas.bar;
            double lambda = double.Parse(HttpContext.Request.Form["lambda"].ToString());
            double cutOff = double.Parse(HttpContext.Request.Form["cutOff"].ToString());
            recaivedDatas=MathOperations.expFilter(recaivedDatas, lambda);
            recaivedDatas= MathOperations.lowPassFilter(recaivedDatas, cutOff);
            List<DataPoint> filtredBar = Graph.fillDataPoints(recaivedDatas.bar, recaivedDatas.numberOfSamples ,(float)recaivedDatas.time);
            List<DataPoint> filtredHammer = Graph.fillDataPoints(recaivedDatas.hammer,recaivedDatas.numberOfSamples , (float)recaivedDatas.time);
            ViewBag.FiltredBar = JsonConvert.SerializeObject(filtredBar);
            ViewBag.FiltredHammer = JsonConvert.SerializeObject(filtredHammer);
            System.Numerics.Complex[] bufferBar = MathOperations.FFTImp(recaivedDatas.bar,(float)recaivedDatas.time, recaivedDatas.numberOfSamples);
            System.Numerics.Complex[] bufferHammer = MathOperations.FFTImp(recaivedDatas.hammer, (float)recaivedDatas.time, recaivedDatas.numberOfSamples);
            double[] fftBar = MathOperations.absComplexToDouble(bufferBar, recaivedDatas.numberOfSamples);
            double[] fftHammer = MathOperations.absComplexToDouble(bufferHammer, recaivedDatas.numberOfSamples);
            float fp = recaivedDatas.numberOfSamples / (float)recaivedDatas.time;
            float freq = fp * recaivedDatas.numberOfSamples;
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
        public IActionResult Final()
        {
            var numberOfPoints = HttpContext.Session.GetInt32("numberOfPoints");
            double[] frfMagnitude;
            float freq = JsonConvert.DeserializeObject<float>(HttpContext.Session.GetString("freq"));
            List<DataPoint> frfMagnitudePoints = new List<DataPoint>();
            List<System.Numerics.Complex[]> frfFinal = JsonConvert.DeserializeObject<List<System.Numerics.Complex[]>>(HttpContext.Session.GetString("frfFinal"));
            for (int i = 0; i < numberOfPoints; i++)
            { 
                frfMagnitude = MathOperations.absComplexToDouble(frfFinal[i], frfFinal[i].Length);
                frfMagnitudePoints = Graph.fillDataPoints(frfMagnitude, (frfFinal[i].Length)/2, freq);
                switch (i)
                {
                    case 0:
                        ViewBag.frf1 = JsonConvert.SerializeObject(frfMagnitudePoints);
                        break;
                    case 1:
                        ViewBag.frf2 = JsonConvert.SerializeObject(frfMagnitudePoints);
                        break;
                    case 2:
                        ViewBag.frf3 = JsonConvert.SerializeObject(frfMagnitudePoints);
                        break;
                    case 3:
                        ViewBag.frf4 = JsonConvert.SerializeObject(frfMagnitudePoints);
                        break;
                    case 4:
                        ViewBag.frf5 = JsonConvert.SerializeObject(frfMagnitudePoints);
                        break;
                   
                }

            }
            
             return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
       
    }
}
