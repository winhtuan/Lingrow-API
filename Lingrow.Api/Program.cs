using System.IdentityModel.Tokens.Jwt;
using Amazon.S3;
using Lingrow.Api.Auth;
using Lingrow.BusinessLogicLayer.Helper;
using Lingrow.BusinessLogicLayer.Interface;
using Lingrow.BusinessLogicLayer.Options;
using Lingrow.BusinessLogicLayer.Service.Auth;
using Lingrow.BusinessLogicLayer.Services.Schedules;
using Lingrow.DataAccessLayer.Data;
using Lingrow.DataAccessLayer.Interface;
using Lingrow.DataAccessLayer.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

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
var cognitoSection = builder.Configuration.GetSection("Cognito");
var authority = cognitoSection["Authority"];
var audience = cognitoSection["Audience"];

if (string.IsNullOrWhiteSpace(authority) || string.IsNullOrWhiteSpace(audience))
{
    throw new InvalidOperationException("Cognito Authority/Audience are not configured.");
}

builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = "Cognito";
        options.DefaultChallengeScheme = "Cognito";
    })
    .AddScheme<AuthenticationSchemeOptions, CognitoJwtHandler>("Cognito", options => { });

builder.Services.AddAuthorization();

// ===== Memory Cache cho JWT handler =====
builder.Services.AddMemoryCache();

// ===== CORS Configuration =====
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowFrontend",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:3000", "http://localhost:3001")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
    );
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ================================
// Repos
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IStudentCardRepo, StudentCardRepo>();
builder.Services.AddScoped<IScheduleRepo, ScheduleRepo>();

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IStudentCardService, StudentCardService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();

// ================================
builder.Services.Configure<AwsOptions>(builder.Configuration.GetSection("Aws"));

// Đăng ký AWS SDK integration
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonS3>();

builder.Services.AddSingleton<S3Helper>();

// Khởi tạo logger helper
LoggerHelper.Configure(builder.Environment.ContentRootPath);

// ================================
var app = builder.Build();

// Forwarded headers từ Nginx / reverse proxy
app.UseForwardedHeaders(
    new ForwardedHeadersOptions
    {
        ForwardedHeaders =
            ForwardedHeaders.XForwardedFor
            | ForwardedHeaders.XForwardedProto
            | ForwardedHeaders.XForwardedHost,
    }
);

// ===== Auto-migrate với retry (quan trọng để tránh lỗi 500 lần đầu) =====
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var db = services.GetRequiredService<AppDbContext>();

    const int maxRetries = 5;
    var delay = TimeSpan.FromSeconds(5);

    for (var attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            logger.LogInformation(
                "Applying database migrations (attempt {Attempt}/{MaxAttempts})...",
                attempt,
                maxRetries
            );

            await db.Database.MigrateAsync();

            logger.LogInformation("Database migrations applied successfully.");
            break; // thành công -> thoát vòng for
        }
        catch (Exception ex)
        {
            if (attempt == maxRetries)
            {
                logger.LogError(
                    ex,
                    "Failed to apply database migrations after {MaxAttempts} attempts. Application will stop.",
                    maxRetries
                );
                throw; // cho container fail nhanh để Docker restart
            }

            logger.LogWarning(
                ex,
                "Database migration failed on attempt {Attempt}/{MaxAttempts}. Retrying in {DelaySeconds} seconds...",
                attempt,
                maxRetries,
                delay.TotalSeconds
            );

            await Task.Delay(delay);
        }
    }
}

// ===== End auto-migrate =====

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection(); // Nginx handle TLS

// ===== CORS =====
app.UseCors("AllowFrontend");

// ===== Middleware auth =====
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
