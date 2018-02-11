using Microsoft.AspNetCore.Http;
using Pidget.Client.DataModels;
using System.Security.Claims;

namespace Pidget.AspNet
{
    public static class UserDataProvider
    {
        public static UserData GetUserData(HttpContext http)
            => http.User != null
                ? new UserData
                {
                    Id = GetId(http.User),
                    UserName = GetUserName(http.User),
                    Email = GetEmail(http.User),
                    IpAddress = GetIpAddress(http)
                }
                : null;

        public static  string GetUserName(ClaimsPrincipal user)
            => user.Identity?.Name
            ?? user.FindFirst(ClaimTypes.Name)?.Value;

        public static string GetEmail(ClaimsPrincipal user)
            => user.FindFirst(ClaimTypes.Email)?.Value;

        public static string GetId(ClaimsPrincipal user)
            => user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public static string GetIpAddress(HttpContext http)
            => GetXForwardedFor(http.Request)
            ?? http.Connection?.RemoteIpAddress?.ToString();

        private static string GetXForwardedFor(HttpRequest req)
        {
            req.Headers?.TryGetValue("X-Forwarded-For",
                out var forwardedFor);

            return forwardedFor;
        }
    }
}
