using LiteDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NightMovie.API.DTO;
using NightMovie.API.Model;
using NightMovie.API.Service.AuthentificationService;
using NightMovie.API.Utils;

namespace NightMovie.API.ApiControllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class SeanceController : ControllerBase
    {

        private readonly ILiteDatabase liteDb;

        private readonly ILogger<SeanceController> _logger;
        private ISeanceService _seanceService;

        public SeanceController(ILogger<SeanceController> logger, ILiteDatabase liteDb, ISeanceService seanceService)
        {
            _logger = logger;
            this.liteDb = liteDb;
            _seanceService = seanceService;
        }

        [HttpGet]
        public IEnumerable<Seance> List()
        {
            ILiteCollection<Seance> col = liteDb.GetCollection<Seance>();
            return col.FindAll().OrderByDescending(x => x.IsOngoing);
        }

        [HttpGet("{id}")]
        public Seance Find(int id)
        {
            string isAdmin = Utils.Utils.GetPayloadFromToken(HttpContext, "isAdmin");
            if (isAdmin == "TRUE")
            {
                ILiteCollection<Seance> col = liteDb.GetCollection<Seance>();
                return col.FindById(id);
            }
            else
            {
                ILiteCollection<Seance> col = liteDb.GetCollection<Seance>();

                return new Seance
                {
                    Users = col.FindById(id).Users,
                    IsOngoing = true,
                    Id = col.FindById(id).Id
                };
            }
        }

        [HttpDelete]
        public void DeleteSeance(int idSeance)
        {
            ILiteCollection<Seance> seances = liteDb.GetCollection<Seance>();
            seances.FindById(idSeance).Film.Seance = null;
            seances.Delete(idSeance);
        }

        /*
        [HttpGet("/testuser")]
        public Dictionary<int, int> testGenerateUser()
        {
            var result = new Dictionary<int, int>();


            var u1 = new User { Id = 1, UserName = "User1", Weight = 1 };
            var u2 = new User { Id = 2, UserName = "User2", Weight = 1 };
            var u3 = new User { Id = 3, UserName = "User3", Weight = 0.5f };
            result.Add(u1.Id, 0);
            result.Add(u2.Id, 0);
            result.Add(u3.Id, 0);
            // Test de la fonction
            var users = new List<User>
            {
                u1,u2,u3
            };
            for (int i = 0; i < 1000000; i++)
            {
                User selectedUser = Utils.Utils.GetRandomUserByWeight(users);
                result[selectedUser.Id] = result[selectedUser.Id]+1;
            }
            return result;
        }*/

        [HttpPost("/api/Seance/Generate")]
        public IActionResult GenerateSeance([FromBody] IEnumerable<User> users)
        {

            try
            {
                ILiteCollection<Seance> seances = liteDb.GetCollection<Seance>();
                ILiteCollection<User> usersCol = liteDb.GetCollection<User>();

                var userRequest = usersCol.Find(x => users.Where(u => u.Id == x.Id).Count() > 0).ToList();

                if (seances.Exists(x => x.IsOngoing == true))
                {
                    return StatusCode(403, new { message = "Une séance est déjà en cours, cloturez là avant d'en lancer une nouvelle." });
                }
                Seance seanceToAdd = new Seance
                {
                    Users = userRequest
                };
                var idSeance = seances.Insert(seanceToAdd);
                try
                {
                    _seanceService.GenerateFilm(idSeance);
                }
                catch (Exception e)
                {
                    seances.Delete(idSeance);
                    throw;
                }
                return Ok();
            }
            catch (Exception e)
            {

                return StatusCode(500, new { message = e.Message });
            }
        }

        [HttpPost("/api/Seance/{id}/Close")]
        public IActionResult CloseSeance(int id)
        {

            //init collections
            ILiteCollection<Seance> seances = liteDb.GetCollection<Seance>();
            ILiteCollection<Film> films = liteDb.GetCollection<Film>();
            ILiteCollection<Categorie> categories = liteDb.GetCollection<Categorie>();
            ILiteCollection<User> users = liteDb.GetCollection<User>();

            //Get seance
            var seance = seances.FindById(id);

            //Update Film
            var film = films.FindById(seance.Film.Id);
            film.Seance = seance;
            films.Update(film);

            //Update categorie
            var categorie = categories.FindById(seance.Categorie.Id);
            categorie.Films.Find(x => x.Id == film.Id).Seance = seance;
            categorie.nbPicked++;
            categories.Update(categorie);

            //Update user
            var user = users.FindById(film.User.Id);
            user.Weight = user.Weight * 0.8f;

            //Update Seance
            seance.IsOngoing = false;
            seances.Update(seance);
            
            return new OkObjectResult(seance);
        }

    }
}



