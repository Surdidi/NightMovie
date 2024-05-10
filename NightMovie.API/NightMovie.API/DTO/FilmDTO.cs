using LiteDB;
using NightMovie.API.Model;

namespace NightMovie.API.DTO
{
    public class FilmDTO
    {
        public string? Nom { get; set; }
        public int? TmdbId { get; set; }
        public int IdCategorie { get; set; }
    }
}
