using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PracaInzynierska.Models;
using System.IO;
using PracaInzynierska.Services;
using Newtonsoft.Json;


namespace PracaInzynierska.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            List<int> list = new List<int>();
            using (var reader = new StreamReader(@"C:\Users\USER\Desktop\dane.csv"))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    int value = Int32.Parse(line);
                    list.Add(value);
                }
            }
            int[] tab = list.ToArray();
            //MathOperations math = new MathOperations();
            //math.FFTImp(tab, 1.321f, 700);

            List<DataPoint> dataPoints = new List<DataPoint>();
            float tp = 1.321f / 700;
            
            for (int i = 0; i < 700; i++)
            {
               
                dataPoints.Add(new DataPoint(tp*i,tab[i]));//czy nie od tp+tp*i
            }

            ViewBag.Time = JsonConvert.SerializeObject(dataPoints);
            MathOperations math = new MathOperations();
            double [] fft = math.FFTImp(tab, 1.321f, 700);
            List<DataPoint> freqPoints = new List<DataPoint>();
            
            float fp = 1 / tp;
            float df = fp / 700;
            for (int i = 0; i < 700; i++)
            {

                freqPoints.Add(new DataPoint(df * i, fft[i]));//czy nie od tp+tp*i
            }

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
