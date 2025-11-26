using System;
using System.Security.Claims;

namespace Lingrow.Api.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            if (user == null)
                throw new UnauthorizedAccessException("User context is missing.");

            var id = user.FindFirst("user_id")?.Value;
            if (string.IsNullOrWhiteSpace(id))
                throw new UnauthorizedAccessException("Missing user_id claim.");

            if (!Guid.TryParse(id, out var guid))
                throw new ArgumentException("Invalid user_id claim.");

            return guid;
        }
    }
}
