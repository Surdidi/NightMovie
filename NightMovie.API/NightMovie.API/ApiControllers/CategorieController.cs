using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NightMovie.API.Database;
using NightMovie.API.Model;
using System.Data.Entity;

namespace NightMovie.API.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategorieController : ControllerBase
    {


        private readonly ILogger<CategorieController> _logger;
        private NightMovieContext _nightMovieContext;

        public CategorieController(ILogger<CategorieController> logger, NightMovieContext nightMovieContext)
        {
            _logger = logger;
            _nightMovieContext = nightMovieContext;
        }

        // GET: api/<FilmController>
        [HttpPost("/api/Categorie/Films")]
        public IEnumerable<Film> FilmsByCategories([FromBody] List<int> ids)
        {
            var listCategories = _nightMovieContext.Films.Where(x => ids.Contains(x.Categorie.ID));
            return listCategories;
        }

        [HttpPost("/api/Categorie")]
        public void Post(Categorie categorie)
        {
            _nightMovieContext.Categories.Update(categorie);
            _nightMovieContext.SaveChanges();
        }


        [HttpGet("/api/Categorie")]
        public IEnumerable<Categorie> Categories()
        {
            return _nightMovieContext.Categories.Include(c => c.Films).ToList();
        }
    }

}
