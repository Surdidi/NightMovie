using LiteDB;

namespace NightMovie.API.Model
{
    public class Seance
    {
        public int Id { get; set; }
        public IEnumerable<User>? Users {get;set;}
        public Film? Film { get; set;}
        [BsonRef("Categorie")]
        public Categorie? Categorie { get; set; }
        public bool IsOngoing { get; set; } = true;


    }
}
