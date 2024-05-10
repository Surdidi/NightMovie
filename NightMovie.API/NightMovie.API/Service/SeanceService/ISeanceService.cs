using LiteDB;
using NightMovie.API.Model;
using System.Security.Cryptography;

namespace NightMovie.API.Service.AuthentificationService
{
    public interface ISeanceService
    {
        public Film GetRandomFilm(int id);

        public Film GenerateFilm(int idSeance);

    }
}
