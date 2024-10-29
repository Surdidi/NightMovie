namespace NightMovie.API.Model
{
    public class Film
    {
        public int ID { get; set; }
        public string? Nom { get; set; }
        public string? TmdbId { get; set; }
        public Categorie Categorie { get; set; }
        public virtual User? User { get; set; }
        public virtual Seance? Seance { get; set; }
    }
}
