using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NightMovie.API.Model;
using NightMovie.API.Service.AuthentificationService;
using NightMovie.API.Database;

namespace NightMovie.API.ApiControllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class SeanceController : ControllerBase
    {
        private readonly ILogger<SeanceController> _logger;
        private ISeanceService _seanceService;
        private NightMovieContext _nightMovieContext;

        public SeanceController(ILogger<SeanceController> logger, ISeanceService seanceService, NightMovieContext context)
        {
            _logger = logger;
            _seanceService = seanceService;
            _nightMovieContext = context;
        }

        [HttpGet]
        public IEnumerable<Seance> List()
        {
            return _nightMovieContext.Seances.OrderByDescending(x => x.IsOngoing);
        }

        [HttpGet("{id}")]
        public Seance Find(int id)
        {
            string isAdmin = Utils.Utils.GetPayloadFromToken(HttpContext, "isAdmin");
            if (isAdmin == "TRUE")
            {
                return _nightMovieContext.Seances.Find(id);
            }
            else
            {
                return new Seance
                {
                    Users = _nightMovieContext.Seances.Find(id).Users,
                    IsOngoing = _nightMovieContext.Seances.Find(id).IsOngoing,
                    ID = _nightMovieContext.Seances.Find(id).ID
                };
            }
        }

        [HttpDelete]
        public void DeleteSeance(int idSeance)
        {
            var seance = _nightMovieContext.Seances.Find(idSeance);
            _nightMovieContext.Films.Find(seance.FilmID).Seance = null;
            _nightMovieContext.Seances.Remove(seance);
            _nightMovieContext.SaveChanges();
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
                var userRequest = _nightMovieContext.Users.Where(x => users.Where(u => u.ID == x.ID).Count() > 0).ToList();

                if (_nightMovieContext.Seances.Any(x => x.IsOngoing == true))
                {
                    return StatusCode(403, new { message = "Une séance est déjà en cours, cloturez là avant d'en lancer une nouvelle." });
                }
                Seance seanceToAdd = new Seance
                {
                    Users = userRequest
                };
                var idSeance = _nightMovieContext.Seances.Add(seanceToAdd);
                try
                {
                    _seanceService.GenerateFilm(seanceToAdd.ID);
                }
                catch (Exception)
                {
                    _nightMovieContext.Seances.Remove(seanceToAdd);
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

            //Get seance
            var seance = _nightMovieContext.Seances.Find(id);

            //Update Film
            var film = _nightMovieContext.Films.Find(seance.Film.ID);
            film.Seance = seance;
            _nightMovieContext.Films.Update(film);

            //Update categorie
            var categorie = _nightMovieContext.Categories.Find(seance.Categorie.ID);
            categorie.nbPicked++;
            _nightMovieContext.Categories.Update(categorie);

            //Update user
            var user = _nightMovieContext.Users.Find(film.User.ID);
            user.Weight = user.Weight * 0.8f;

            //Update Seance
            seance.IsOngoing = false;
            _nightMovieContext.Seances.Update(seance);

            _nightMovieContext.SaveChanges();

            return new OkObjectResult(seance);
        }

    }
}



