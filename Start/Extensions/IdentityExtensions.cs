using Domain.Entities.SecurityEntities;
using Microsoft.AspNetCore.Identity;
using Persistence.Data;

namespace Takwene.Api.Extensions;

public static class IdentityExtensions
{
    public static IServiceCollection AddTakweneIdentity(this IServiceCollection services)
    {
        services.AddIdentity<User, IdentityRole>(options =>
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

        return services;
    }
}