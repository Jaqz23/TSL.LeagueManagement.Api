using TSL.Core.Application;
using TSL.Infrastructure.Persistence;
using TSL.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddPersistenceInfrastructure(builder.Configuration);
builder.Services.AddApplicationLayer();

builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerExtension();
builder.Services.AddApiVersioningExtension();
builder.Services.AddCorsExtension(builder.Configuration);
builder.Services.AddControllersExtension();
builder.Services.AddEndpointsApiExplorer();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TSL API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();


if (app.Environment.IsDevelopment()) 
{
    app.UseCors("AllowAll");
}
else 
{
    app.UseCors("ProductionPolicy");
}

  
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
