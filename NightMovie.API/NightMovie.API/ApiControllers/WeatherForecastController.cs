//using LiteDB;
//using Microsoft.AspNetCore.Mvc;
//using NightMovie.API.Model;

//namespace NightMovie.API.Controllers
//{
//    [ApiController]
//    [Route("[controller]")]
//    public class WeatherForecastController : ControllerBase
//    {
//        private static readonly string[] Summaries = new[]
//        {
//        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
//        };
//        private readonly ILiteDatabase liteDb;

//        private readonly ILogger<WeatherForecastController> _logger;

//        public WeatherForecastController(ILogger<WeatherForecastController> logger, ILiteDatabase liteDb)
//        {
//            _logger = logger;
//            this.liteDb = liteDb;
//        }

//        [HttpGet(Name = "GetWeatherForecast")]
//        public IEnumerable<WeatherForecast> Get()
//        {
//            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
//            {
//                Date = DateTime.Now.AddDays(index),
//                TemperatureC = Random.Shared.Next(-20, 55),
//                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
//            })
//            .ToArray();
//        }

//        [HttpPost(Name = "Test")]
//        public async void Test()
//        {
            
//            var test = new User { UserName = "TOTO", password = "password" };
//            var Categorie = new Categorie { Nom = "Action" };
//            var film = new Film { Nom = "rambo", User = test, Categorie = Categorie };
//            /*await sqlController.SaveUserAsync(test);
//            await sqlController.SaveCategorieAsync(Categorie);
//            await sqlController.SaveFilmAsync(film);
//            */

//            var db = new LiteDatabase("Data/MyDB.db");
//            var a = liteDb.GetCollection<User>();
//            var users = db.GetCollection<User>();
//            var categories = db.GetCollection<Categorie>();
//            var films = db.GetCollection<Film>();

//            users.Insert(test);
//            categories.Insert(Categorie);
//            films.Insert(film);
//        }



//    }
//}