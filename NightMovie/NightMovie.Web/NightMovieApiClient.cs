using NightMovie.API.Model;
using NightMovie.API.DTO;
using NightMovie.Web.Services;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace NightMovie.Web
{
    public class NightMovieApiClient(HttpClient httpClient, AuthService auth)
    {


        public async Task<string> getToken()
        {
            return await auth.GetToken();
        }
        public async Task Login(LoginOrCreateDTO login) 
        {
            StringContent content = new StringContent(JsonSerializer.Serialize(login), Encoding.UTF8, "application/json");
                   var response = await httpClient.PostAsync("/api/Authentification/Login", content);
             var responseContent = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            await auth.NotifyUserAuthentication(responseContent);
        }

        public async Task<IEnumerable<Categorie>> GetCategories()
        {
            try
            {
                using var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/api/Categorie");

                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await getToken());

                var response = await httpClient.SendAsync(requestMessage);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadFromJsonAsync<IEnumerable<Categorie>>();
                return responseContent;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task AddFilm(FilmDTO film)
        {
            StringContent content = new StringContent(JsonSerializer.Serialize(film), Encoding.UTF8, "application/json");
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Film");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await getToken());
            requestMessage.Content = content;
            var response = await httpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<Film>> GetMovies()
        {
            try
            {
                using var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/api/Film");

                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await getToken());

                var response = await httpClient.SendAsync(requestMessage);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadFromJsonAsync<IEnumerable<Film>>();
                return responseContent;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
