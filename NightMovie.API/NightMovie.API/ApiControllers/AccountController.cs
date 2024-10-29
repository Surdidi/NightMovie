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
    public class AccountController : ControllerBase
    {

        private readonly ILogger<AccountController> _logger;
        private IAuthentificationService authenficationService;
        private NightMovieContext _dbContext;

        public AccountController(ILogger<AccountController> logger, NightMovieContext dbContext, IAuthentificationService authentificationService)
        {
            _logger = logger;
            this._dbContext = dbContext;
            this.authenficationService = authentificationService;
        }

        [HttpPost]
        public void ChangePassword([FromBody] string password)
        {

            string userId = Utils.Utils.GetPayloadFromToken(HttpContext, "userId");
            var userRequest = _dbContext.Users.Find(Int32.Parse(userId));
            userRequest.password = authenficationService.HashPassword(password);
            _dbContext.Users.Update(userRequest);
            _dbContext.SaveChanges();
        }

        [HttpGet("{id}")]
        public User Get(int id)
        {
            return _dbContext.Users.Find(id);
        }

        [HttpGet]
        public IEnumerable<User> GetAll()
        {
            return _dbContext.Users;
        }
    }
}
