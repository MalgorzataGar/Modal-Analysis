using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PracaInzynierska.Models;
using PracaInzynierska.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace PracaInzynierska.Controllers
{
    public class HomeController : Controller
    {
        int numberOfTestsPerPoint;
        public readonly IConnectionService ConnectionService;
        public readonly IGraph Graph;
        public readonly IMathOperations MathOperations;
        public List<RecaivedData> recaivedDatas;
        int currentMeasure;
        int currentPoint;

        public HomeController(IConnectionService connectionService, IGraph graph, IMathOperations mathOperations)
        {
            ConnectionService = connectionService;
            Graph = graph;
            MathOperations = mathOperations;
            currentMeasure = -1;
            currentPoint = 1;
        }
        public IActionResult Index()
        {
            ViewBag.currentMeasure = currentMeasure;
            ViewBag.currentPoint = currentPoint;
            return View();
        }
        public IActionResult AnalizeData()
        {
            numberOfTestsPerPoint = Int32.Parse(HttpContext.Request.Form["numberOfTestsPerPoint"].ToString());
            if (currentMeasure == -1) currentMeasure = 0;
            else currentMeasure++;
            string filename;//nazwa z data
            string str = "{'filename': 'pomiar2.txt'}";
            JObject configJson = JObject.Parse(str);
            ConnectionService.HandleConnection(configJson);
            recaivedDatas.Add(ConnectionService.GetDataFromString());
            
            double[] bar_raw;
            double[] hammer_raw;
            float time = 6.192f;
            int numberOfPoints = 1500;
            Graph.dataFromFile(out bar_raw, out hammer_raw);
            double[] bar = MathOperations.normalization(bar_raw, numberOfPoints);
            double[] hammer = MathOperations.normalization(hammer_raw, numberOfPoints);

            List<DataPoint> pointsListBar = Graph.fillDataPoints(bar, numberOfPoints, time);
            List<DataPoint> pointsListHammer = Graph.fillDataPoints(hammer, numberOfPoints, time);

            ViewBag.TimeBar = JsonConvert.SerializeObject(pointsListBar);
            ViewBag.TimeHammer = JsonConvert.SerializeObject(pointsListHammer);
            //tyle w tej funkcji, w view uzytkownik podaje zakresy 
            return View();
        }

        public IActionResult Filtration()
        {
            double[] bar = recaivedDatas[currentMeasure].bar;
            double[] hammer = recaivedDatas[currentMeasure].bar; ;
            float time = 6.192f;
            int numberOfPoints = 1500;

            recaivedDatas[currentMeasure]=MathOperations.expFilter(recaivedDatas[currentMeasure], 21.3);
            recaivedDatas[currentMeasure] = MathOperations.lowPassFilter(recaivedDatas[currentMeasure], 2);
            System.Numerics.Complex[] bufferBar = MathOperations.FFTImp(recaivedDatas[currentMeasure].bar,(float)recaivedDatas[currentMeasure].time, recaivedDatas[currentMeasure].numberOfSamples);
            System.Numerics.Complex[] bufferHammer = MathOperations.FFTImp(recaivedDatas[currentMeasure].hammer, (float)recaivedDatas[currentMeasure].time, recaivedDatas[currentMeasure].numberOfSamples);
            double[] fftBar = MathOperations.absComplexToDouble(bufferBar, numberOfPoints);
            double[] fftHammer = MathOperations.absComplexToDouble(bufferHammer, numberOfPoints);
            float fp = numberOfPoints / time;
            List<DataPoint> freqPointsBar = Graph.fillDataPoints(fftBar, (numberOfPoints / 2), fp);
            ViewBag.FreqBar = JsonConvert.SerializeObject(freqPointsBar);
            List<DataPoint> freqPointsHammer = Graph.fillDataPoints(fftHammer, (numberOfPoints / 2), fp);
            ViewBag.FreqHammer = JsonConvert.SerializeObject(freqPointsHammer);
            //oblicz frf i na dole guzik zaakceptuj itd
            //tu zwraca wykrey przefiltrowane i FFT i jesli jest ok to zapisuje punkt 
            //zapisuj frf
            //zwieksz wartosc punktu
            //przenies do funkcji analize data i pobierz od uzytkownika
            return View();
        }
        public IActionResult Accept()
        {
            //pobierz od uzytkownika czy akceptuje 
            //jesli tak sprawdz czy juz zadana ilosc punktow
            //jak nie
            //prejdz do analizy 
            //jak tak to usrednianie frf
            //jesli nie akceptuje to current -- i wtedy analiza
            
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
