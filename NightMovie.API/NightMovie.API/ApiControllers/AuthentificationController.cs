using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using NightMovie.API.DTO;
using NightMovie.API.Model;
using NightMovie.API.Service.AuthentificationService;
using NightMovie.API.Database;

namespace NightMovie.API.ApiControllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthentificationController : ControllerBase
    {
        private NightMovieContext _nightMovieContext { get; set; }
        private readonly ILogger<AuthentificationController> _logger;
        private IAuthentificationService authenficationService;

        public AuthentificationController(ILogger<AuthentificationController> logger, IAuthentificationService authentificationService, NightMovieContext nightMovieContext)
        {
            _logger = logger;
            _nightMovieContext = nightMovieContext;
            this.authenficationService = authentificationService;
        }

        // POST api/<FilmController>
        [HttpPost]
        [EnableCors("ApiCorsPolicy")]
        public string Login([FromBody] LoginOrCreateDTO login)
        {
            var user = _nightMovieContext.Users.Where(x => x.UserName.ToUpper() == login.Username.ToUpper()).FirstOrDefault();
            authenficationService.ComparePassword(user, login.Password);
            var tmp = authenficationService.GenerateJsonWebToken(user);
            return tmp;
        }

    }
}
