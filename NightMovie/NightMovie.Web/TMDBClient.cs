using NightMovie.Web.Models;
using System.Net.Http;
using System.Text.Json;
namespace NightMovie.Web
{
    public class TMDBClient(HttpClient httpClient)
    {
        private const string apiKey = "2e7e507d33b685e1e504a18975294495";
        private const string baseUrl = "https://api.themoviedb.org/3/movie/";

        public async Task<List<Movie>> SearchMovieByNameAsync(string movieName)
        {
            var test = new HttpClient();
            var url = $"/search/movie?api_key={apiKey}&language=fr-FR&query={Uri.EscapeDataString(movieName)}";
            var urltest = $"https://api.themoviedb.org/3/search/movie?language=fr-FR&page=1&query={movieName}&api_key={apiKey}";
            var response = await test.GetAsync(urltest);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var searchResult = JsonSerializer.Deserialize<TmdbFilm>(content, options);
                return searchResult!.Results;
            }

            return new List<Movie>();
        }

        public async Task<MovieDetail> GetMovieAsync(string tmdbId)
        {
            var test = new HttpClient();
            var url = $"{baseUrl}{tmdbId}?api_key={apiKey}&language=fr-FR";
            var response = await test.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var movie = JsonSerializer.Deserialize<MovieDetail>(content);
                return movie;
            }
            else
            {
                throw new Exception("Can't reach TMDB");
            }
        }

        public async Task<List<MovieDetail>> GetMoviesAsync(List<string> tmdbIds)
        {
            var movies = new List<MovieDetail>();

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

    }
}
