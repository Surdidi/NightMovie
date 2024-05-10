using NightMovie.API.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using LiteDB;
using NightMovie.API.Utils;

namespace NightMovie.API.Service.AuthentificationService
{
    public class SeanceService : ISeanceService
    {
        private readonly IConfiguration Configuration;
        private readonly ILiteDatabase liteDb;


        public SeanceService(IConfiguration configuration, ILiteDatabase liteDb)
        {
            Configuration = configuration;
            this.liteDb = liteDb;

        }

        public static User GetRandomUserByWeight(IList<User> users, Categorie categorie)
        {
            Random _rand = new Random();

            //Check that user have add film in this categorie
            var userPossible = users.Where(u => categorie.Films.Where(f => f.User.Id == u.Id).Count() > 0);

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
            //Get information database
            ILiteCollection<Categorie> categories = liteDb.GetCollection<Categorie>();
            ILiteCollection<Seance> seances = liteDb.GetCollection<Seance>();

            //retrieve seance
            var seance = seances.FindById(id);

            //Get categorie
            var minValue = categories.Find(x => x.Films != null).Min(a => a.nbPicked);
            var categoriesPossible = categories.Find(x => x.nbPicked == minValue && x.Films.Where(f => f.Seance == null).Count() > 0);
            if(categoriesPossible.Count() == 0)
            {
                throw new Exception("Il n'y a plus de film à proposer pour cette séance.");
            }
            var categorie = categoriesPossible.RandomElement();
            

            //Get user
            bool verif = true;
            User selectedUser = null;


            //Verify that user have add film in this categorie
            IEnumerable<Film> filmTmp = categorie.Films.Where(film => seance.Users.Count(user => user.Id == film.User.Id) > 0);
            selectedUser = GetRandomUserByWeight(seance.Users.ToList(),categorie);

            //Get film
            Film film = categorie.Films.Where(x => x.User.Id == selectedUser.Id && x.Seance == null).RandomElement();
            return film;
        }

        public Film GenerateFilm(int idSeance)
        {
            ILiteCollection<Seance> seances = liteDb.GetCollection<Seance>();
            var seance = seances.FindById(idSeance);
            var film = GetRandomFilm(idSeance);
            seance.Film = film;
            seance.Categorie = film.Categorie;
            seances.Update(seance);
            return film;
        }

    }
}
