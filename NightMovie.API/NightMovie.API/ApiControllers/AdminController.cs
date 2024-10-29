using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NightMovie.API.Database;
using NightMovie.API.DTO;
using NightMovie.API.Model;
using NightMovie.API.Service.AuthentificationService;

namespace NightMovie.API.ApiControllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Policy = "Admin")]
    public class AdminController : ControllerBase
    {


        private readonly ILogger<AdminController> _logger;
        private IAuthentificationService authenficationService;
        private NightMovieContext _nightMovieContext;

        public AdminController(ILogger<AdminController> logger, IAuthentificationService authentificationService, NightMovieContext nightMovieContext)
        {
            _logger = logger;
            this.authenficationService = authentificationService;
            _nightMovieContext = nightMovieContext;
        }

        [HttpPost]
        public void CreateUser(LoginOrCreateDTO userToAdd)
        {
            string userId = Utils.Utils.GetPayloadFromToken(HttpContext, "userId");
            var userRequest = _nightMovieContext.Users.Find(userId);
            if (userRequest.IsAdmin)
            {
                var user = new User
                {
                    UserName = userToAdd.Username,
                    password = authenficationService.HashPassword(userToAdd.Password),
                    IsAdmin = false
                };
                _nightMovieContext.Users.Add(user);
                _nightMovieContext.SaveChanges();
            }
            else
            {
                throw new UnauthorizedAccessException();
            }

        }


        /*[HttpPost]
        public void CreateUserTmp(LoginOrCreateDTO userToAdd)
        {
            ILiteCollection<User> col = liteDb.GetCollection<User>();
                var user = new User
                {
                    UserName = userToAdd.Username,
                    password = authenficationService.HashPassword(userToAdd.Password),
                    IsAdmin = true
                };
                col.Upsert(user);
        }*/

        [HttpPost]
        public void DeleteUser([FromBody] int id)
        {
            string userId = Utils.Utils.GetPayloadFromToken(HttpContext, "userId");
            var userRequest = _nightMovieContext.Users.Find(userId);
            if (userRequest.IsAdmin)
            {
                _nightMovieContext.Users.Remove(_nightMovieContext.Users.Find(id));
                _nightMovieContext.SaveChanges();
            }
            else
            {
                throw new UnauthorizedAccessException();
            }

        }
    }
}
