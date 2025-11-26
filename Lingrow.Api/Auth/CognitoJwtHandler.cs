using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Lingrow.BusinessLogicLayer.Helper;
using Lingrow.DataAccessLayer.Interface;
using Microsoft.AspNetCore.Authentication;
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

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authHeader = Request.Headers["Authorization"].ToString();

            if (
                string.IsNullOrWhiteSpace(authHeader)
                || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
            )
            {
                // Không có token => để [Authorize] xử lý 401
                return AuthenticateResult.NoResult();
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();

            if (string.IsNullOrWhiteSpace(token))
            {
                return AuthenticateResult.NoResult();
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);

                var claims = new List<Claim>();

                // parse payload thủ công (giữ đúng như code anh đang có)
                try
                {
                    var parts = token.Split('.');
                    if (parts.Length >= 2)
                    {
                        var payload = parts[1];
                        var padding = payload.Length % 4;
                        if (padding > 0)
                        {
                            payload += new string('=', 4 - padding);
                        }

                        var payloadBytes = Convert.FromBase64String(payload);
                        var payloadJson = System.Text.Encoding.UTF8.GetString(payloadBytes);

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

                // --- Lấy sub, email, token_use ---
                var sub =
                    claims.FirstOrDefault(c => c.Type == "sub")?.Value
                    ?? jwt.Subject
                    ?? claims.FirstOrDefault(c => c.Type == "cognito:username")?.Value
                    ?? claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                var email = claims.FirstOrDefault(c => c.Type == "email")?.Value;
                var tokenUse = claims.FirstOrDefault(c => c.Type == "token_use")?.Value;

                // Thêm NameIdentifier nếu chưa có
                if (
                    !string.IsNullOrEmpty(sub)
                    && !claims.Any(c => c.Type == ClaimTypes.NameIdentifier)
                )
                {
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, sub));
                }

                // --- Tra DB lấy UserId và add claim user_id ---
                if (!string.IsNullOrWhiteSpace(sub))
                {
                    try
                    {
                        var userRepo = Context.RequestServices.GetRequiredService<IUserRepo>();
                        var user = await userRepo.GetByCognitoSubAsync(sub);
                        if (user != null)
                        {
                            claims.Add(new Claim("user_id", user.UserId.ToString()));
                        }
                        else
                        {
                            LoggerHelper.Warn($"No UserAccount found for sub={sub}");
                        }
                    }
                    catch (Exception ex)
                    {
                        LoggerHelper.Error($"Failed to resolve user_id from sub: {ex.Message}");
                    }
                }

                LoggerHelper.Info(
                    $"Cognito JWT authenticated - token_use: {tokenUse ?? "null"}, sub: {sub ?? "null"}, email: {email ?? "null"}"
                );

                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex);
                return AuthenticateResult.Fail("Invalid JWT");
            }
        }
    }
}
