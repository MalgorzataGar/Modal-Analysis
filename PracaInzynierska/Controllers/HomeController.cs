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
        public List<RecaivedData> recaivedDatas;//przemysl sens listy
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
            string filename = "myFile_" + DateTime.Now.ToShortTimeString() + ".txt";//time and date?
            string str = "{'filename': '" + filename + "'}";
            JObject configJson = JObject.Parse(str);
            //ConnectionService.HandleConnection(configJson);
            recaivedDatas.Add(ConnectionService.GetDataFromString());
            double[] bar_raw = recaivedDatas[currentMeasure].bar;
            double[] hammer_raw = recaivedDatas[currentMeasure].hammer;
            Graph.dataFromFile(out bar_raw, out hammer_raw);
            double[] bar = MathOperations.normalization(bar_raw, recaivedDatas[currentMeasure].numberOfSamples);
            double[] hammer = MathOperations.normalization(hammer_raw, recaivedDatas[currentMeasure].numberOfSamples);
            recaivedDatas[currentMeasure].bar = bar;
            recaivedDatas[currentMeasure].hammer = hammer;
            List<DataPoint> pointsListBar = Graph.fillDataPoints(bar, recaivedDatas[currentMeasure].numberOfSamples, (float)recaivedDatas[currentMeasure].time);
            List<DataPoint> pointsListHammer = Graph.fillDataPoints(hammer, recaivedDatas[currentMeasure].numberOfSamples, (float)recaivedDatas[currentMeasure].time);
            ViewBag.TimeBar = JsonConvert.SerializeObject(pointsListBar);
            ViewBag.TimeHammer = JsonConvert.SerializeObject(pointsListHammer);
            //tyle w tej funkcji, wyswietl dwa wykresy czasowe a potem pobierz dwie dane lamb
            return View();
        }

        public IActionResult Filtration()
        {
            double[] bar = recaivedDatas[currentMeasure].bar;
            double[] hammer = recaivedDatas[currentMeasure].bar;
            double lambda = double.Parse(HttpContext.Request.Form["lambda"].ToString());
            double cutOff = double.Parse(HttpContext.Request.Form["cutOff"].ToString());
            recaivedDatas[currentMeasure]=MathOperations.expFilter(recaivedDatas[currentMeasure], lambda);
            recaivedDatas[currentMeasure] = MathOperations.lowPassFilter(recaivedDatas[currentMeasure], cutOff);
            System.Numerics.Complex[] bufferBar = MathOperations.FFTImp(recaivedDatas[currentMeasure].bar,(float)recaivedDatas[currentMeasure].time, recaivedDatas[currentMeasure].numberOfSamples);
            System.Numerics.Complex[] bufferHammer = MathOperations.FFTImp(recaivedDatas[currentMeasure].hammer, (float)recaivedDatas[currentMeasure].time, recaivedDatas[currentMeasure].numberOfSamples);
            double[] fftBar = MathOperations.absComplexToDouble(bufferBar, recaivedDatas[currentMeasure].numberOfSamples);
            double[] fftHammer = MathOperations.absComplexToDouble(bufferHammer, recaivedDatas[currentMeasure].numberOfSamples);
            float fp = recaivedDatas[currentMeasure].numberOfSamples / (float)recaivedDatas[currentMeasure].time;
            List<DataPoint> freqPointsBar = Graph.fillDataPoints(fftBar, (recaivedDatas[currentMeasure].numberOfSamples / 2), fp);
            ViewBag.FreqBar = JsonConvert.SerializeObject(freqPointsBar);
            List<DataPoint> freqPointsHammer = Graph.fillDataPoints(fftHammer, (recaivedDatas[currentMeasure].numberOfSamples / 2), fp);
            ViewBag.FreqHammer = JsonConvert.SerializeObject(freqPointsHammer);
            //oblicz frf i na dole guzik zaakceptuj itd
            //tu zwraca wykrey przefiltrowane i FFT i jesli jest ok to zapisuje punkt 
            //zapisuj frf
            return View();
        }
        public IActionResult Accept()
        {
            //pobierz od uzytkownika czy akceptuje 
            //oblicz funkcje pzrejscia i daj ja do tablicy funkcji pzrejscia
            //jesli tak sprawdz czy juz zadana ilosc punktow
            //jak nie
            //prejdz do analizy 
            //jak tak to usrednianie frf i list.clear
            //jesli nie akceptuje to current -- i wtedy analiza i pamietaj usunac ostatnie list
            
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
