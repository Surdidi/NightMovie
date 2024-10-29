using System.ComponentModel.DataAnnotations;

namespace NightMovie.API.DTO
{
    public class LoginOrCreateDTO
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
