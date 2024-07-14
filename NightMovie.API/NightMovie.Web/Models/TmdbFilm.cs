namespace NightMovie.Web.Models
{
    public class TmdbFilm
    {
        public List<Movie> Results { get; set; }
    }

    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ReleaseDate { get; set; } // au format YYYY-MM-DD
        public string PosterPath { get; set; } // c'est souvent un chemin partiel
        public string Overview { get; set; }
    }

}
