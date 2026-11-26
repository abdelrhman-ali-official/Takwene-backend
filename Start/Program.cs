using Takwene.Api.Extensions;
using Takwene.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplicationServices(builder.Configuration, builder.Environment);

var app = builder.Build();
await app.InitializeDatabaseAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.EnablePersistAuthorization();
    });
}

app.UseMiddleware<GlobalErrorHandlingMiddleware>();
app.UseCors(CorsExtensions.PolicyName);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
