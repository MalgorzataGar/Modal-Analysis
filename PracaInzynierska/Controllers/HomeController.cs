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
            double[] tab = mygraph.dataFromFile();
            List < DataPoint > pointsList = mygraph.fillDataPoints(tab, 700, 1.327f);

            ViewBag.Time = JsonConvert.SerializeObject(pointsList);
            MathOperations math = new MathOperations();
            System.Numerics.Complex[] buffer = math.FFTImp(tab, 1.321f, 700);
            double[] fft = math.absComplexToDouble(buffer, 700);
            float fp = 700 / 1.321f;
            List<DataPoint> freqPoints = mygraph.fillDataPoints(fft, 700, fp);
            
            ViewBag.Freq = JsonConvert.SerializeObject(freqPoints);

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
