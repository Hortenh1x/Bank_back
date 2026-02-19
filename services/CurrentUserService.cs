using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Bank_back.Services
{
    public interface ICurrentUserService
    {
        int GetUserId();
    }

    public sealed class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public int GetUserId()
        {
            var user = httpContextAccessor.HttpContext?.User;
            var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim))
            {
                throw new UnauthorizedAccessException("User id not found in the session");
            }

            if (!int.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("User id in the session is invalid");
            }

            return userId;
        }
    }
}
