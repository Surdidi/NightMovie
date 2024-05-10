using LiteDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NightMovie.API.DTO;
using NightMovie.API.Model;
using NightMovie.API.Service.AuthentificationService;
using System.Xml.Linq;

namespace NightMovie.API.ApiControllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthentificationController : ControllerBase
    {
        private readonly ILiteDatabase liteDb;

        private readonly ILogger<AuthentificationController> _logger;
        private IAuthentificationService authenficationService;

        public AuthentificationController(ILogger<AuthentificationController> logger, ILiteDatabase liteDb, IAuthentificationService authentificationService)
        {
            _logger = logger;
            this.liteDb = liteDb;
            this.authenficationService = authentificationService;
        }

        // POST api/<FilmController>
        [HttpPost]
        public string Login([FromBody] LoginOrCreateDTO login)
        {
            ILiteCollection<User> col = liteDb.GetCollection<User>();
            var user = col.Find(x => x.UserName.ToUpper() == login.Username.ToUpper()).First();
            authenficationService.ComparePassword(user, login.Password);
            return authenficationService.GenerateJsonWebToken(user);
        }        
        
    }
}
