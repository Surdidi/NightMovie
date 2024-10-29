using NightMovie.API.Model;
using NightMovie.API.Service.AuthentificationService;
namespace NightMovie.API.Database
{
    public static class NightMovieDBInitializer
    {
        public static void Initialize(NightMovieContext context, IAuthentificationService auth)
        {
            context.Database.EnsureCreated();

            var users = new List<User>
            {
                new User { UserName = "admin", password = auth.HashPassword("admin"), IsAdmin = true },
                new User { UserName = "Dylan", password = auth.HashPassword("6/deux=TROIS"), IsAdmin = false },
                new User { UserName = "Marie", password = auth.HashPassword("6/deux=TROIS"), IsAdmin = false },
                new User { UserName = "Anthony", password = auth.HashPassword("6/deux=TROIS"), IsAdmin = false },
                new User { UserName = "Ayleen", password = auth.HashPassword("6/deux=TROIS"), IsAdmin = false },
                new User { UserName = "David", password = auth.HashPassword("6/deux=TROIS"), IsAdmin = false },
                new User { UserName = "Romain", password = auth.HashPassword("6/deux=TROIS"), IsAdmin = false },
                new User { UserName = "Laurie", password = auth.HashPassword("6/deux=TROIS"), IsAdmin = false },
            };

            // Look for any students.
            if (context.Users.Any())
            {
                return;
            }

            var categories = new List<Categorie>
            {
                new Categorie { Nom = "Action - Guerre - Histoire", Description = "Films d'actions, guerre et histoire", nbMaxByPerson = 3, nbPicked = 0, Films = []},
                new Categorie { Nom = "Comédie", Description = "Films comiques", nbMaxByPerson = 3, nbPicked = 0 , Films = []},
                new Categorie { Nom = "Drame - Romance", Description = "Films dramatiques", nbMaxByPerson = 3, nbPicked = 0, Films = [new Film { Nom = "toto"}] },
                new Categorie { Nom = "Horreur -  Science Fiction", Description = "Films d'horreur", nbMaxByPerson = 3, nbPicked = 0, Films = [] },
                new Categorie { Nom = "Nanar", Description = "Nanars", nbMaxByPerson = 1, nbPicked = 0 , Films = []},
            };
            categories.ForEach(c => context.Categories.Add(c));
            context.SaveChanges();

            users.ForEach(u => context.Users.Add(u));
            context.SaveChanges();
        }
    }
}
