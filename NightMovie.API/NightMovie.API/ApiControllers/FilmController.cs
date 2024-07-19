using LiteDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using NightMovie.API.Controllers;
using NightMovie.Model.DTO;
using NightMovie.API.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NightMovie.API.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FilmController : ControllerBase
    {

        private readonly ILiteDatabase liteDb;

        private readonly ILogger<FilmController> _logger;

        public FilmController(ILogger<FilmController> logger, ILiteDatabase liteDb)
        {
            _logger = logger;
            this.liteDb = liteDb;
        }

        // GET: api/<FilmController>
        [HttpGet]
        public IEnumerable<Film> Get()
        {
            ILiteCollection<Film> col = liteDb.GetCollection<Film>();
            return col.FindAll();
        }

        // GET api/<FilmController>/5
        [HttpGet("{id}")]
        public Film Get(int id)
        {
            ILiteCollection<Film> col = liteDb.GetCollection<Film>();
            return col.FindById(id);
        }

        // POST api/<FilmController>
        [HttpPost]
        public IActionResult Post([FromBody] FilmDTO value)
        {
            ILiteCollection<Film> col = liteDb.GetCollection<Film>();
            ILiteCollection<Categorie> categories = liteDb.GetCollection<Categorie>();
            var categ = categories.FindById(value.IdCategorie);

            string userId = Utils.Utils.GetPayloadFromToken(HttpContext, "userId");
            var userRequest = liteDb.GetCollection<User>().Find(x => x.Id == Int32.Parse(userId)).First();
            if(value.IdCategorie == null)
            {
                return BadRequest(new { message = "La catégorie est obligatoire." });
            }
            if (col.Exists(x => x.TmdbId == value.TmdbId))
            {
                return Conflict(new { message = "Le film a déjà été ajouté par vous ou un autre utilisateur." });

            }
            if (col.Find(x => x.Categorie.Id == value.IdCategorie && x.User == userRequest && x.Seance == null).Count() >= categ.nbMaxByPerson)
            {
                return StatusCode(403, new { message = "Vous avez dépassé votre quota de films pour cette catégorie." });
            }

            var film = new Film
            {
                Nom = value.Nom,
                TmdbId = value.TmdbId,
                Categorie = categ,
                User = userRequest
            };
            col.Insert(film);
            if(categ.Films != null)
            {
                categ.Films.Add(film);
            }
            else
            {
                categ.Films = new List<Film>
                {
                    film
                };
            }
            categories.Upsert(categ);
            return Ok();
        }

        // DELETE api/<FilmController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            ILiteCollection<Film> films = liteDb.GetCollection<Film>();
            var film = films.FindById(id);
            ILiteCollection<Categorie> categories = liteDb.GetCollection<Categorie>();
            var categ = categories.FindById(film.Categorie.Id);
            categ.Films.Remove(film);
            categories.Upsert(categ);
            films.Delete(id);
        }

        // POST api/<FilmController>/Vote/5
        [HttpPost("/api/Vote/{id}")]
        public void Vote(int id, int note)
        {
            ILiteCollection<Film> col = liteDb.GetCollection<Film>();

            string userId = Utils.Utils.GetPayloadFromToken(HttpContext, "userId");
            var userRequest = liteDb.GetCollection<User>().Find(x => x.Id == Int32.Parse(userId)).First();
            var film = col.FindById(id);
            var addNote = new Vote
            {
                Note = note,
                User = userRequest
            };
            col.Upsert(film);
        }        
    }
}
