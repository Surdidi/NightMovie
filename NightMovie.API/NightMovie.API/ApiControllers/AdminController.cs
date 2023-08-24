using LiteDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        private readonly ILiteDatabase liteDb;

        private readonly ILogger<AdminController> _logger;
        private IAuthentificationService authenficationService;

        public AdminController(ILogger<AdminController> logger, ILiteDatabase liteDb, IAuthentificationService authentificationService)
        {
            _logger = logger;
            this.liteDb = liteDb;
            this.authenficationService = authentificationService;
        }

        [HttpPost]
        public void CreateUser(LoginOrCreateDTO userToAdd)
        {
            ILiteCollection<User> col = liteDb.GetCollection<User>();

            string userId = Utils.Utils.GetPayloadFromToken(HttpContext, "userId");
            var userRequest = col.Find(x => x.Id == Int32.Parse(userId)).First();
            if (userRequest.IsAdmin)
            {
                var user = new User
                {
                    UserName = userToAdd.Username,
                    password = authenficationService.HashPassword(userToAdd.Password),
                    IsAdmin = false
                };
                col.Upsert(user);
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
            ILiteCollection<User> col = liteDb.GetCollection<User>();

            string userId = Utils.Utils.GetPayloadFromToken(HttpContext, "userId");
            var userRequest = col.Find(x => x.Id == Int32.Parse(userId)).First();
            if (userRequest.IsAdmin)
            {
                col.Delete(id);
            }
            else
            {
                throw new UnauthorizedAccessException();
            }

        }
    }
}
