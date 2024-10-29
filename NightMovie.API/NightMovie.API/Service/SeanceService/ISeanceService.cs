using NightMovie.API.Model;

namespace NightMovie.API.Service.AuthentificationService
{
    public interface ISeanceService
    {
        public Film GetRandomFilm(int id);

        public Film GenerateFilm(int idSeance);

    }
}
