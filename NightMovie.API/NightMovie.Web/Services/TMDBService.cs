using NightMovie.Web.Models;
using System.Text.Json;
using NightMovie.Web.Services.Contracts;

namespace NightMovie.API.Services
{
    public class TMDBService : ITMDBService

    {
        private readonly HttpClient _httpClient;
        private const string apiKey = "2e7e507d33b685e1e504a18975294495";
        private const string baseUrl = "https://api.themoviedb.org/3/movie/";

        public TMDBService(HttpClient httpClient) 
        { 
            _httpClient = httpClient;
        }

        public async Task<Movie> GetMovieAsync(int tmdbId)
        {
            var url = $"{baseUrl}{tmdbId}?api_key={apiKey}&language=fr-FR";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var movie = JsonSerializer.Deserialize<Movie>(content);
                return movie;
            }
            else
            {
                throw new Exception("Can't reach TMDB");
            }
        }

        public async Task<List<Movie>> GetMoviesAsync(List<int> tmdbIds)
        {
            var movies = new List<Movie>();

            foreach (var id in tmdbIds)
            {
                var movie = await GetMovieAsync(id);
                if (movie != null)
                {
                    movies.Add(movie);
                }
            }

            return movies;
        }

        public async Task<List<Movie>> SearchMovieByNameAsync(string movieName)
        {
            var url = $"{baseUrl}search/movie?api_key={apiKey}&language=fr-FR&query={Uri.EscapeDataString(movieName)}";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var searchResult = JsonSerializer.Deserialize<TmdbFilm>(content);
                return searchResult?.Results;
            }

            return new List<Movie>();
        }
    }

}
