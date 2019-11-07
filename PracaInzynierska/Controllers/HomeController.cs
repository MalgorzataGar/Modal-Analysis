using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PracaInzynierska.Models;
using PracaInzynierska.Interfaces;
using Newtonsoft.Json;


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
<<<<<<< HEAD
=======
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
>>>>>>> 4b5e463ee9eb6c82b6551839dec9661d5e5fb597

            ConnectionService = connectionService;
            Graph = graph;
            MathOperations = mathOperations;
        }
        public IActionResult Index()
        {
            //get data from viewer
            //set number of tests
            //uruchom
            //nt from view
            //start pzrenosi do fukcji ospowiadajacej za connection  
            // dopisz instukcje
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
