using Domain.Contracts;
using Domain.Entities.SecurityEntities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Persistence;
using Persistence.Data;
using Persistence.Repositories;
using Services;
using Services.Abstractions;
using Shared.SecurityModels;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var jwtSection = builder.Configuration.GetSection("JwtOptions");
var jwtOptions = jwtSection.Get<JwtOptions>() ?? throw new InvalidOperationException("JwtOptions configuration is required.");
if (string.IsNullOrWhiteSpace(jwtOptions.SecretKey) || Encoding.UTF8.GetByteCount(jwtOptions.SecretKey) < 32)
    throw new InvalidOperationException("Configure JwtOptions:SecretKey with a secret of at least 32 bytes through User Secrets or an environment variable.");
if (string.IsNullOrWhiteSpace(jwtOptions.Issuer) || string.IsNullOrWhiteSpace(jwtOptions.Audience) || jwtOptions.DurationInDays is <= 0 or > 7)
    throw new InvalidOperationException("JwtOptions Issuer, Audience, and a DurationInDays value between 1 and 7 must be configured.");

builder.Services.AddControllers().AddApplicationPart(typeof(Presentation.AssemblyReference).Assembly);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<JwtOptions>(jwtSection);
builder.Services.AddDbContext<StoreContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection")));
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
}).AddEntityFrameworkStores<StoreContext>().AddDefaultTokenProviders();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidAudience = jwtOptions.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
        NameClaimType = ClaimTypes.Email,
        RoleClaimType = ClaimTypes.Role,
        ClockSkew = TimeSpan.FromMinutes(1)
    });
builder.Services.AddAuthorization();
builder.Services.AddScoped<IUnitOFWork, UnitOfWork>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IArtistService, ArtistService>();
builder.Services.AddScoped<ITrackService, TrackService>();
builder.Services.AddScoped<IDSPService, DSPService>();
builder.Services.AddScoped<ITrackDistributionService, TrackDistributionService>();
builder.Services.AddScoped<IDbInitializer, DbInitializer>();

var app = builder.Build();
await InitializeDatabaseAsync(app);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<Takwene.Api.Middlewares.GlobalErrorHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

static async Task InitializeDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var initializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseBootstrap");

    try
    {
        if (app.Environment.IsProduction())
        {
            logger.LogInformation("Verifying database initialization; production migrations are managed by deployment.");
            await initializer.VerifyInitializedAsync();
        }
        else
        {
            await initializer.InitializeAsync();
        }
    }
    catch (Exception exception)
    {
        logger.LogCritical(exception, "Database bootstrap failed. The API will not start.");
        throw;
    }
}
