using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PracaInzynierska.Models;
using PracaInzynierska.Services;
using Newtonsoft.Json;


namespace PracaInzynierska.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            Graph mygraph = new Graph();
            double[] bar_raw;
            double[] hammer_raw;
            float time = 6.192f;
            int numberOfPoints = 1500;
            MathOperations math = new MathOperations();
            mygraph.dataFromFile(out bar_raw, out hammer_raw);
            double[] bar = math.normalization(bar_raw, numberOfPoints);
            double[] hammer = math.normalization(hammer_raw, numberOfPoints);

            List < DataPoint > pointsListBar = mygraph.fillDataPoints(bar, numberOfPoints, time);
            List<DataPoint> pointsListHammer = mygraph.fillDataPoints(hammer, numberOfPoints, time);
            
            ViewBag.TimeBar = JsonConvert.SerializeObject(pointsListBar);
            ViewBag.TimeHammer = JsonConvert.SerializeObject(pointsListHammer);
            
            System.Numerics.Complex[] bufferBar = math.FFTImp(bar, time, numberOfPoints);
            System.Numerics.Complex[] bufferHammer = math.FFTImp(hammer, time, numberOfPoints);
            double[] fftBar = math.absComplexToDouble(bufferBar, numberOfPoints);
            double[] fftHammer = math.absComplexToDouble(bufferHammer, numberOfPoints);
            float fp = numberOfPoints / time;
            List<DataPoint> freqPointsBar = mygraph.fillDataPoints(fftBar, (numberOfPoints/2), fp);
            List<DataPoint> filtredBar = math.expFilter(freqPointsBar, 0.01, numberOfPoints / 2);
            ViewBag.FreqBar = JsonConvert.SerializeObject(filtredBar);
            List<DataPoint> freqPointsHammer = mygraph.fillDataPoints(fftHammer, (numberOfPoints / 2) , fp);
            List<DataPoint> filtredHammer = math.lowPassFilter(freqPointsHammer, 21.3, numberOfPoints / 2);
            ViewBag.FreqHammer = JsonConvert.SerializeObject(filtredHammer);

            return View();
        }
        
        public IActionResult SubmitData()
        {
            //guzik pobierz dane,

            return View("Index");
        }
        [HttpPost]
        //public IActionResult GetResponse([FromBody] JObject receivedData)
        //{
        //    //pobranie danych od uzytkownika, zapis do bazy danych? 
        //    //potrzebny serwis fourier
        //    return View("Response");
        //}
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
