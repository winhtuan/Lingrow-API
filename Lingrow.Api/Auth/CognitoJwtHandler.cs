using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Lingrow.Api.Auth
{
    public class CognitoJwtHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        [Obsolete]
        public CognitoJwtHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            System.Text.Encodings.Web.UrlEncoder encoder,
            ISystemClock clock
        )
            : base(options, logger, encoder, clock) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authHeader = Request.Headers["Authorization"].ToString();

            if (
                string.IsNullOrWhiteSpace(authHeader)
                || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
            )
            {
                // Không có token => để [Authorize] xử lý 401
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();

            if (string.IsNullOrWhiteSpace(token))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();

                // Chỉ decode, KHÔNG validate chữ ký (dùng cho dev)
                var jwt = handler.ReadJwtToken(token);

                var claims = jwt.Claims.ToList();

                // Cố gắng lấy sub từ nhiều nguồn
                var sub =
                    claims.FirstOrDefault(c => c.Type == "sub")?.Value
                    ?? jwt.Subject
                    ?? claims.FirstOrDefault(c => c.Type == "cognito:username")?.Value
                    ?? claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                var email = claims.FirstOrDefault(c => c.Type == "email")?.Value;
                var tokenUse = claims.FirstOrDefault(c => c.Type == "token_use")?.Value;

                // Nếu chưa có claim "sub" nhưng đã tìm được sub fallback => thêm vào claims
                if (!string.IsNullOrEmpty(sub) && !claims.Any(c => c.Type == "sub"))
                {
                    claims.Add(new Claim("sub", sub));
                }

                // Thêm NameIdentifier cho tiện dùng ClaimTypes.NameIdentifier
                if (
                    !string.IsNullOrEmpty(sub)
                    && !claims.Any(c => c.Type == ClaimTypes.NameIdentifier)
                )
                {
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, sub));
                }

                Logger.LogInformation(
                    "CognitoJwtHandler authenticated. token_use={TokenUse}, sub={Sub}, email={Email}",
                    tokenUse ?? "(null)",
                    sub ?? "(null)",
                    email ?? "(null)"
                );

                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "CognitoJwtHandler failed to parse token");
                return Task.FromResult(AuthenticateResult.Fail("Invalid JWT"));
            }
        }
    }
}
