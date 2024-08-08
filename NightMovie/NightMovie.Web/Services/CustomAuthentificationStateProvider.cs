namespace NightMovie.Web.Services
{
    using Blazored.LocalStorage;
    using Microsoft.AspNetCore.Components.Authorization;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class AuthService
    {
        private readonly ILocalStorageService _localStorageService;

        public AuthService(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        public async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorageService.GetItemAsync<string>("authToken");

            var identity = string.IsNullOrEmpty(token) ? new ClaimsIdentity() : new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "User") }, "jwt");

            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }

        public void NotifyUserAuthentication(string token)
        {
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "User") }, "jwt");
            var user = new ClaimsPrincipal(identity);
        }

        public void NotifyUserLogout()
        {
            var identity = new ClaimsIdentity();
            var user = new ClaimsPrincipal(identity);
        }

        public async Task<string> GetToken()
        {
            return await _localStorageService.GetItemAsync<string>("authToken");
        }

        public async Task<bool> IsTokenExpired()
        {
            var token = await GetToken();
            if (string.IsNullOrEmpty(token))
                return false;

            var tokenHandler = new JwtSecurityTokenHandler();

            if (!tokenHandler.CanReadToken(token))
                return false;

            var jwtToken = tokenHandler.ReadJwtToken(token);

            // Check the expiration claim
            var expClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "exp");
            if (expClaim == null)
                return false;

            // Get the expiration time in Unix format
            var expUnix = long.Parse(expClaim.Value);
            var expirationTime = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;

            // Check if the token is expired
            return expirationTime < DateTime.UtcNow;
        }
    }

}
