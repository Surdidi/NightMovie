using LiteDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NightMovie.API.Model;
using NightMovie.API.Utils;

namespace NightMovie.API.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SeanceController : ControllerBase
    {

        private readonly ILiteDatabase liteDb;

        private readonly ILogger<SeanceController> _logger;

        public SeanceController(ILogger<SeanceController> logger, ILiteDatabase liteDb)
        {
            _logger = logger;
            this.liteDb = liteDb;
        }

        [HttpGet]
        public IEnumerable<Seance> Get()
        {
            ILiteCollection<Seance> col = liteDb.GetCollection<Seance>();
            return col.FindAll();
        }

        [HttpGet("{id}")]
        public Seance Find(int id)
        {
            ILiteCollection<Seance> col = liteDb.GetCollection<Seance>();
            return col.FindById(id);
        }

        [HttpGet("/{id}/users")]
        public IEnumerable<User> FindUsers(int id)
        {
            ILiteCollection<Seance> col = liteDb.GetCollection<Seance>();
            return col.FindById(id).Users;
        }

        [HttpGet("/{id}/film")]
        public Film FindFilm(int id)
        {
            ILiteCollection<Seance> col = liteDb.GetCollection<Seance>();
            return col.FindById(id).Film;
        }

        [HttpPost()]
        public void AddUser([FromBody]Seance seance)
        {
            ILiteCollection<Seance> seances = liteDb.GetCollection<Seance>();
            seances.Insert(seance);
        }

        [HttpPost("/{idSeance}/users/{idUser}")]
        public void AddUser(int idSeance,int idUser)
        {
            ILiteCollection<Seance> seances = liteDb.GetCollection<Seance>();
            User user = liteDb.GetCollection<User>().FindById(idUser);
            var seance = seances.FindById(idSeance);
            seance.Users.Append(user);
            seances.Update(seance);
        }

        [HttpPost("/{idSeance}/generatefilm")]
        public void GenerateFilm(int idSeance)
        {
            ILiteCollection<Seance> seances = liteDb.GetCollection<Seance>();
            ILiteCollection<Film> films = liteDb.GetCollection<Film>();
            var seance = seances.FindById(idSeance);
            IEnumerable<Film> possibleFilm = films.Find(f => f.Seance == null && seance.Users.Contains(f.User));
            var film = possibleFilm.RandomElement();
            seance.Film = film;
            film.Seance = seance;
            films.Update(film);
            seances.Update(seance);
        }

        [HttpPost("/{idSeance}/generatefilm/{idCategorie}")]
        public void GenerateFilmByCategorie(int idSeance, int idCategorie)
        {
            ILiteCollection<Seance> seances = liteDb.GetCollection<Seance>();
            ILiteCollection<Film> films = liteDb.GetCollection<Film>();
            ILiteCollection<Categorie> categories = liteDb.GetCollection<Categorie>();
            var categorie = categories.FindById(idCategorie);
            var seance = seances.FindById(idSeance);
            IEnumerable<Film> possibleFilm = films.Find(f => f.Seance == null && seance.Users.Contains(f.User) && f.Categorie == categorie);
            var film = possibleFilm.RandomElement();
            seance.Film = film;
            film.Seance = seance;
            films.Update(film);
            seances.Update(seance);
        }
    }
}
