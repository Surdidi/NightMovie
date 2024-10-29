namespace NightMovie.API.Model
{
    public class Categorie
    {
        public int ID { get; set; }
        public string? Nom { get; set; }
        public string? Description { get; set; }
        public int nbMaxByPerson { get; set; } = 3;
        public int nbPicked { get; set; } = 0;
        public ICollection<Film> Films { get; set; }

    }
}
