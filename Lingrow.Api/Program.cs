using Lingrow.BusinessLogicLayer.Interface;
using Lingrow.DataAccessLayer.Data;
using Lingrow.DataAccessLayer.Interface;
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

var connStr =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? builder.Configuration["ConnectionStrings:DefaultConnection"];

// docker-pgadmin
// "DefaultConnection": "Host=127.0.0.1;Port=5555;Database=AppDb;Username=rootuser;Password=strongpassword;Pooling=true;Ssl Mode=Disable;Trust Server Certificate=true"
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(connStr));

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IUserService, UserService>();

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

// Nếu chạy sau Nginx (HTTP nội bộ), bạn có thể TẮT HTTPS redirection:
#if !DEBUG
// app.UseHttpsRedirection(); // Nginx handle TLS
#endif

app.UseAuthorization();

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

// "ConnectionStrings": {
//     "DefaultConnection": "Host=localhost;Port=5432;Database=Lingrow;Username=postgres;Password=root"
//   },
