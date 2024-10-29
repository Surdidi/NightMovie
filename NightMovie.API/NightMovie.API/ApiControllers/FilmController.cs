using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NightMovie.API.DTO;
using NightMovie.API.Model;
using NightMovie.API.Database;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NightMovie.API.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FilmController : ControllerBase
    {


        private readonly ILogger<FilmController> _logger;
        private NightMovieContext _dbContext;
        public FilmController(ILogger<FilmController> logger, NightMovieContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        // GET: api/<FilmController>
        [HttpGet]
        public IEnumerable<Film> Get()
        {
            return _dbContext.Films.ToList();
        }

        // GET api/<FilmController>/5
        [HttpGet("{id}")]
        public Film Get(int id)
        {
            return _dbContext.Films.Find(id);
        }

        // POST api/<FilmController>
        [HttpPost]
        public IActionResult Post([FromBody] FilmDTO value)
        {
            Categorie ? categ = _dbContext.Categories.Include(x => x.Films).First(a => a.ID == value.IdCategorie);

            string userId = Utils.Utils.GetPayloadFromToken(HttpContext, "userId");
            var userRequest = _dbContext.Users.Find(Int32.Parse(userId));
            if(value.IdCategorie == null)
            {
                return BadRequest(new { message = "La catégorie est obligatoire." });
            }
            if (_dbContext.Films.Any(x => x.TmdbId == value.TmdbId))
            {
                return Conflict(new { message = "Le film a déjà été ajouté par vous ou un autre utilisateur." });

            }
            if (_dbContext.Films.Where(x => x.Categorie.ID == value.IdCategorie && x.User == userRequest && x.Seance == null).Count() >= categ.nbMaxByPerson)
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
            var toto = _dbContext.Categories.Include(x => x.Films).First(x => x.ID == value.IdCategorie);
            _dbContext.Categories.Include(x => x.Films).First(x => x.ID == value.IdCategorie).Films.Add(film);
            _dbContext.Films.Add(film);
            _dbContext.SaveChanges();
            return Ok();
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _dbContext.Films.Remove(_dbContext.Films.Find(id));
            _dbContext.SaveChanges();
        }

        [HttpPost("/api/Vote/{id}")]
        public void Vote(int id, int note)
        {

            string userId = Utils.Utils.GetPayloadFromToken(HttpContext, "userId");
            var userRequest = _dbContext.Users.Find(Int32.Parse(userId));
            var film = _dbContext.Films.Find(id);
            var addNote = new Vote
            {
                Note = note,
                User = userRequest
            };
            _dbContext.SaveChanges();
        }        
    }
}
