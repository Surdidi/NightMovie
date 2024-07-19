using NightMovie.Web.Models;

namespace NightMovie.Web.Services.Contracts
{
    public interface ITMDBService
    {
        Task<Movie> GetMovieAsync(int tmdbId);
        Task<List<Movie>> GetMoviesAsync(List<int> tmdbIds);
        Task<List<Movie>> SearchMovieByNameAsync(string movieName);
    }
}
