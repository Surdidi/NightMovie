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
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly ILiteDatabase liteDb;

        private readonly ILogger<AccountController> _logger;
        private IAuthentificationService authenficationService;

        public AccountController(ILogger<AccountController> logger, ILiteDatabase liteDb, IAuthentificationService authentificationService)
        {
            _logger = logger;
            this.liteDb = liteDb;
            this.authenficationService = authentificationService;
        }

        [HttpPost]
        public void ChangePassword([FromBody] string password)
        {
            ILiteCollection<User> col = liteDb.GetCollection<User>();

            string userId = Utils.Utils.GetPayloadFromToken(HttpContext, "userId");
            var userRequest = col.Find(x => x.Id == Int32.Parse(userId)).First();
            userRequest.password = authenficationService.HashPassword(password);
            col.Upsert(userRequest);
        }

        [HttpGet("{id}")]
        public User Get(int id)
        {
            ILiteCollection<User> col = liteDb.GetCollection<User>();
            return col.FindById(id);
        }

        [HttpGet]
        public IEnumerable<User> GetAll()
        {
            return liteDb.GetCollection<User>().FindAll();
        }
    }
}
