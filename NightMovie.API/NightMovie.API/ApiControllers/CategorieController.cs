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
        [HttpPost("/api/Categorie/Films")]
        public IEnumerable<Film> FilmsByCategories([FromBody] List<int> ids)
        {
            ILiteCollection<Film> categories = liteDb.GetCollection<Film>();
            var listCategories = categories.Find(x => ids.Contains(x.Categorie.Id));

            return listCategories;
        }

        [HttpPost("/api/Categorie")]
        public void Post(Categorie categorie)
        {
            ILiteCollection<Categorie> categories = liteDb.GetCollection<Categorie>();
            categories.Upsert(categorie);
        }


        [HttpGet("/api/Categorie")]
        public IEnumerable<Categorie> Categories()
        {
            ILiteCollection<Categorie> categories = liteDb.GetCollection<Categorie>();
            return categories.FindAll();
        }
    }

}
