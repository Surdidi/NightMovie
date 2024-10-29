using NightMovie.API.Model;
using NightMovie.API.Utils;
using NightMovie.API.Database;

namespace NightMovie.API.Service.AuthentificationService
{
    public class SeanceService : ISeanceService
    {
        private readonly IConfiguration Configuration;
        private NightMovieContext _nightMovieContext;

        public SeanceService(IConfiguration configuration, NightMovieContext nightMovieContext)
        {
            Configuration = configuration;
            _nightMovieContext = nightMovieContext;
        }

        public User GetRandomUserByWeight(IList<User> users, Categorie categorie)
        {
            Random _rand = new Random();

            //Check that user have add film in this categorie
            var userPossible = users
            .Where(user => _nightMovieContext.Films
                .Any(film => film.User.ID == user.ID && film.Categorie.ID == categorie.ID && film.Seance.ID == null))
            .ToList();

            // Calculez la somme totale des poids
            float totalWeight = userPossible.Sum(u => u.Weight);

            // Choisissez un nombre aléatoire entre 0 et la somme totale des poids
            float randomValue = (float)(_rand.NextDouble() * totalWeight);

            float cumulativeWeight = 0;
            foreach (var user in userPossible)
            {
                cumulativeWeight += user.Weight;
                if (randomValue < cumulativeWeight)
                    return user;
            }
            return userPossible.First();
        }


        public Film GetRandomFilm(int id)
        {
            //retrieve seance
            var seance = _nightMovieContext.Seances.Find(id);

            //Get categorie
            var minValue = _nightMovieContext.Categories.Where(x => x.Films != null).Min(a => a.nbPicked);
            var categoriesPossible = _nightMovieContext.Categories.Where(x => x.nbPicked == minValue && x.Films.Where(f => f.Seance == null).Count() > 0);
            if (categoriesPossible.Count() == 0)
            {
                throw new Exception("Il n'y a plus de film à proposer pour cette séance.");
            }
            var categorie = categoriesPossible.RandomElement();


            //Get user
            bool verif = true;
            User selectedUser = null;


            //Verify that user have add film in this categorie
            IEnumerable<Film> filmTmp = categorie.Films.Where(film => seance.Users.Count(user => user.ID == film.User.ID) > 0);
            selectedUser = GetRandomUserByWeight(seance.Users.ToList(), categorie);

            //Get film
            Film film = categorie.Films.Where(x => x.User.ID == selectedUser.ID && x.Seance == null).RandomElement();
            _nightMovieContext.SaveChanges();
            return film;
        }

        public Film GenerateFilm(int idSeance)
        {
            var seance = _nightMovieContext.Seances.Find(idSeance);
            var film = GetRandomFilm(idSeance);
            seance.Film = film;
            seance.Categorie = film.Categorie;
            _nightMovieContext.Seances.Update(seance);
            _nightMovieContext.SaveChanges();
            return film;
        }

    }
}
