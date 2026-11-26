namespace Takwene.Api.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddTakweneSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        return services;
    }
}