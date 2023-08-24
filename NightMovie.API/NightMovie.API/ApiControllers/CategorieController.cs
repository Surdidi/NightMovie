using LiteDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NightMovie.API.Model;

namespace NightMovie.API.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategorieController : ControllerBase
    {

        private readonly ILiteDatabase liteDb;

        private readonly ILogger<CategorieController> _logger;

        public CategorieController(ILogger<CategorieController> logger, ILiteDatabase liteDb)
        {
            _logger = logger;
            this.liteDb = liteDb;
        }

        // GET: api/<FilmController>
        [HttpGet("/api/Categorie/Film/{id}")]
        public IEnumerable<Film> Categories(int id)
        {
            ILiteCollection<Categorie> categories = liteDb.GetCollection<Categorie>();
            return categories.FindById(id).Films;
        }

        [HttpPost("/api/Categorie/Film/{id}")]
        public void Post(Categorie categorie)
        {
            ILiteCollection<Categorie> categories = liteDb.GetCollection<Categorie>();
            categories.Upsert(categorie);
        }
    }

}
