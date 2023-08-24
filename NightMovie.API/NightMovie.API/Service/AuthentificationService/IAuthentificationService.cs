using NightMovie.API.Model;
using System.Security.Cryptography;

namespace NightMovie.API.Service.AuthentificationService
{
    public interface IAuthentificationService
    {

        public string HashPassword(string? password);
        public void ComparePassword(User user, string? password);
        public string GenerateJsonWebToken(User user);
    }
}
