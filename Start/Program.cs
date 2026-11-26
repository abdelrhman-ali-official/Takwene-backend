using Takwene.Api.Extensions;
using Takwene.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();
await app.InitializeDatabaseAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalErrorHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
