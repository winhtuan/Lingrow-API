using Lingrow.BusinessLogicLayer.Auth;
using Lingrow.BusinessLogicLayer.Helper;
using Lingrow.BusinessLogicLayer.Interface;
using Lingrow.BusinessLogicLayer.Options;
using Lingrow.BusinessLogicLayer.Service.Auth;
using Lingrow.DataAccessLayer.Data;
using Lingrow.DataAccessLayer.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình port theo environment
if (builder.Environment.IsDevelopment())
{
    // Local dev: chạy ở port 5189
    builder.WebHost.UseUrls("http://localhost:5189");
}
else
{
    // Docker / production: listen mọi IP trong container ở port 5000
    builder.WebHost.UseKestrel().UseUrls("http://0.0.0.0:5000");
}

var connStr = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connStr))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
}

builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection("AuthOptions"));

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(connStr));

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// ===== JWT + Cognito config =====
var jwtSection = builder.Configuration.GetSection("Jwt");
var authority = jwtSection["Authority"];
var audience = jwtSection["Audience"];

builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        // Cognito issuer
        options.Authority = authority;
        options.Audience = audience;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = authority,

            ValidateAudience = true,
            ValidAudience = audience,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2), // cho phép lệch thời gian nhẹ
        };

        // Optional: log lỗi token
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = ctx =>
            {
                Console.WriteLine($"JWT auth failed: {ctx.Exception.Message}");
                return Task.CompletedTask;
            },
        };
    });

builder.Services.AddAuthorization();

// ================================

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IUserService, UserService>();

// Khởi tạo logger: Logs sẽ nằm trong {ContentRoot}/Logs
LoggerHelper.Configure(builder.Environment.ContentRootPath);

var app = builder.Build();

// Forwarded headers từ Nginx
app.UseForwardedHeaders(
    new ForwardedHeadersOptions
    {
        ForwardedHeaders =
            ForwardedHeaders.XForwardedFor
            | ForwardedHeaders.XForwardedProto
            | ForwardedHeaders.XForwardedHost,
    }
);

// Auto-migrate (dev/CI)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Nếu chạy sau Nginx (HTTP nội bộ), có thể tắt HTTPS redirection
#if !DEBUG
// app.UseHttpsRedirection(); // Nginx handle TLS
#endif

// ===== Thứ tự middleware auth =====
app.UseAuthentication();
app.UseAuthorization();

// ================================

app.MapControllers();

app.MapGet(
    "/db/ping",
    async (AppDbContext db) =>
    {
        await db.Database.ExecuteSqlRawAsync("select 1;");
        return Results.Ok("DB OK");
    }
);

app.Run();
