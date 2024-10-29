namespace NightMovie.API.Model
{
    public class Seance
    {
        public int ID { get; set; }
        public IEnumerable<User>? Users {get;set;}

        public int FilmID { get; set; }
        public Film? Film { get; set;}

        public int CategorieID { get; set; }
        public Categorie? Categorie { get; set; }
        public bool IsOngoing { get; set; } = true;
    }
}
