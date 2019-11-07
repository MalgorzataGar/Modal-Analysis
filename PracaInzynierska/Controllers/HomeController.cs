using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PracaInzynierska.Models;
using PracaInzynierska.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace PracaInzynierska.Controllers
{
    public class HomeController : Controller
    {
        int nT;
        int nP;
        //stuktura danych zawierajaca 5 tablicy pomiarow bar i hammer z jednego punktu
        //tablica fft ostateczne i 5 tablic da
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
            string str = "{'filename': 'pomiar2.txt'}";
            JObject configJson = JObject.Parse(str);
            ConnectionService.HandleConnection(configJson);
            return View();
        }
        public IActionResult AnalizeData()
        {
            //tu pobierz dane od uzytkownika
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

            System.Numerics.Complex[] bufferBar = MathOperations.FFTImp(bar, time, numberOfPoints);
            System.Numerics.Complex[] bufferHammer = MathOperations.FFTImp(hammer, time, numberOfPoints);
            double[] fftBar = MathOperations.absComplexToDouble(bufferBar, numberOfPoints);
            double[] fftHammer = MathOperations.absComplexToDouble(bufferHammer, numberOfPoints);
            float fp = numberOfPoints / time;
            List<DataPoint> freqPointsBar = Graph.fillDataPoints(fftBar, (numberOfPoints / 2), fp);
            List<DataPoint> filtredBar = MathOperations.expFilter(freqPointsBar, 0.01, numberOfPoints / 2);
            ViewBag.FreqBar = JsonConvert.SerializeObject(filtredBar);
            List<DataPoint> freqPointsHammer = Graph.fillDataPoints(fftHammer, (numberOfPoints / 2), fp);
            List<DataPoint> filtredHammer = MathOperations.lowPassFilter(freqPointsHammer, 21.3, numberOfPoints / 2);
            ViewBag.FreqHammer = JsonConvert.SerializeObject(filtredHammer);

            //tu bedzie opcja wlaczenia filtracji i podania zakresu
            //zaakceptuj dane 


            return View();
        }

        public IActionResult Filtration()
        {
            //tu zwraca wykrey przefiltrowane i FFT i jesli jest ok to zapisuje punkt 
            //zapisuj frf
            //zwieksz wartosc punktu
            //przenies do funkcji analize data i pobierz od uzytkownika
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
