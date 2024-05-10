using LiteDB;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace NightMovie.API.Model
{
    [Table("Categorie")]
    public class Categorie
    {
        public int Id { get; set; }
        public string? Nom { get; set; }
        public string? Description { get; set; }
        public List<Film>? Films { get; set; }
        public int nbMaxByPerson { get; set; } = 3;

        public int nbPicked { get; set; } = 0;
    }
}
