using System.Security.Claims;

namespace todo_web_api.Extensions
{
    public static class ClaimsExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return user.Claims.FirstOrDefault(x =>
                x.Type.Equals("https://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname") ||
                x.Type.Equals(ClaimTypes.GivenName) ||
                x.Type.Equals(ClaimTypes.Name))?.Value ?? string.Empty;
        }
    }
}
