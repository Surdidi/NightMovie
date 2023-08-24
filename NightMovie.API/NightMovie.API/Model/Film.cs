using LiteDB;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace NightMovie.API.Model
{
    [Table("Film")]
    public class Film
    {
        public int Id { get; set; }
        public string? Nom { get; set; }
        public string? ImdbId { get; set; }

        [BsonRef("Categorie")]
        public Categorie? Categorie { get; set; }

        [BsonRef("User")]
        public User? User { get; set; }
        public List<Vote>? Votes { get; set; }

        public Seance? Seance { get; set; }
    }
}
