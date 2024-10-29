using NightMovie.API.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;


namespace NightMovie.API.Service.AuthentificationService
{
    public class AuthenficationService : IAuthentificationService
    {
        private const int HashSize = 16;
        private const int Iterations = 100000;
        private const int ByteSize = 20;
        private readonly IConfiguration Configuration;


        public AuthenficationService(IConfiguration configuration)
        {
            Configuration = configuration;

        }

        public string HashPassword(string? password)
        {
            byte[] salt = new byte[HashSize];
            RandomNumberGenerator.Create().GetBytes(salt);

            Rfc2898DeriveBytes pbkdf2 = new(password ?? throw new ArgumentNullException(nameof(password)), salt, Iterations);
            byte[] hash = pbkdf2.GetBytes(ByteSize);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, HashSize);
            Array.Copy(hash, 0, hashBytes, HashSize, ByteSize);
            string savedPasswordHash = Convert.ToBase64String(hashBytes);
            return savedPasswordHash;
        }

        public void ComparePassword(User user, string? password)
        {
            if (user == null) throw new UnauthorizedAccessException();
            /* Extract the bytes */
            byte[] hashBytes = Convert.FromBase64String(user.password ?? throw new UnauthorizedAccessException());
            /* Get the salt */
            byte[] salt = new byte[HashSize];
            Array.Copy(hashBytes, 0, salt, 0, HashSize);
            /* Compute the hash on the password the user entered */
            Rfc2898DeriveBytes pbkdf2 = new(password ?? throw new UnauthorizedAccessException(), salt, Iterations);
            byte[] hash = pbkdf2.GetBytes(ByteSize);
            /* Compare the results */
            for (int i = 0; i < ByteSize; i++)
            {
                if (hashBytes[i + HashSize] != hash[i])
                {
                    throw new UnauthorizedAccessException();
                }
            }
        }
        
        public string GenerateJsonWebToken(User user)
        {
            var Claims = new List<Claim>
            {
                new Claim("userId",user.ID.ToString()),
                new Claim("isAdmin",user.IsAdmin.ToString().ToUpper())
            };

            var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Authentification_SignatureKey"]));

            var Token = new JwtSecurityToken(
                "nightMovie.API",
                "nightMovie.API",
                Claims,
                expires: DateTime.Now.AddDays(30.0),
                signingCredentials: new SigningCredentials(Key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(Token);
        }
         
    }
}
