using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Lingrow.BusinessLogicLayer.Helper;
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
                var jwt = handler.ReadJwtToken(token);

                // Parse claims manually from payload JSON because JwtSecurityTokenHandler
                // doesn't parse all claims correctly (especially those with colons like cognito:username)
                var claims = new List<Claim>();
                try
                {
                    var parts = token.Split('.');
                    if (parts.Length >= 2)
                    {
                        var payload = parts[1];
                        // Add padding if needed
                        var padding = payload.Length % 4;
                        if (padding > 0)
                        {
                            payload += new string('=', 4 - padding);
                        }
                        var payloadBytes = Convert.FromBase64String(payload);
                        var payloadJson = System.Text.Encoding.UTF8.GetString(payloadBytes);

                        // Parse JSON and convert to claims
                        using var doc = System.Text.Json.JsonDocument.Parse(payloadJson);
                        foreach (var prop in doc.RootElement.EnumerateObject())
                        {
                            var claimType = prop.Name;
                            var claimValue = prop.Value.ValueKind switch
                            {
                                System.Text.Json.JsonValueKind.String => prop.Value.GetString(),
                                System.Text.Json.JsonValueKind.Number => prop.Value.GetRawText(),
                                System.Text.Json.JsonValueKind.True => "true",
                                System.Text.Json.JsonValueKind.False => "false",
                                System.Text.Json.JsonValueKind.Array => prop.Value.GetRawText(),
                                System.Text.Json.JsonValueKind.Object => prop.Value.GetRawText(),
                                _ => prop.Value.GetRawText(),
                            };

                            if (!string.IsNullOrEmpty(claimValue))
                            {
                                claims.Add(new Claim(claimType, claimValue));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LoggerHelper.Warn($"Failed to parse JWT payload manually: {ex.Message}");
                    claims = [.. jwt.Claims];
                }

                // Lấy sub từ claims
                var sub =
                    claims.FirstOrDefault(c => c.Type == "sub")?.Value
                    ?? jwt.Subject
                    ?? claims.FirstOrDefault(c => c.Type == "cognito:username")?.Value
                    ?? claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                var email = claims.FirstOrDefault(c => c.Type == "email")?.Value;
                var tokenUse = claims.FirstOrDefault(c => c.Type == "token_use")?.Value;

                // Thêm NameIdentifier claim nếu chưa có
                if (
                    !string.IsNullOrEmpty(sub)
                    && !claims.Any(c => c.Type == ClaimTypes.NameIdentifier)
                )
                {
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, sub));
                }

                LoggerHelper.Info(
                    $"Cognito JWT authenticated - token_use: {tokenUse ?? "null"}, sub: {sub ?? "null"}, email: {email ?? "null"}"
                );

                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex);
                return Task.FromResult(AuthenticateResult.Fail("Invalid JWT"));
            }
        }
    }
}
