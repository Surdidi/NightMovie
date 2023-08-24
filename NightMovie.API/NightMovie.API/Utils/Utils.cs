using System.IdentityModel.Tokens.Jwt;

namespace NightMovie.API.Utils
{
    public static class Utils
    {
        public static string GetPayloadFromToken(HttpContext? context, string payload)
        {
            string? token = context?.Request.Headers["Authorization"].ToString().Split(" ")[1];
            return new JwtSecurityTokenHandler().ReadJwtToken(token).Payload[payload].ToString()!;
        }

        public static T RandomElement<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.RandomElementUsing<T>(new Random());
        }

        public static T RandomElementUsing<T>(this IEnumerable<T> enumerable, Random rand)
        {
            int index = rand.Next(0, enumerable.Count());
            return enumerable.ElementAt(index);
        }

    }
}
