using BolsilloFeliz.Application;
using BolsilloFeliz.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

var origenesPermitidos = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy("Blazor", policy => policy
        .WithOrigins(origenesPermitidos)
        .AllowAnyHeader()
        .AllowAnyMethod());
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();
app.UseCors("Blazor");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();


//using BolsilloFeliz.Infrastructure;
//using BolsilloFeliz.Infrastructure.Persistence;
//using Microsoft.EntityFrameworkCore;
//using Scalar.AspNetCore;


//var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddInfrastructure(builder.Configuration);

//builder.Services.AddDbContext<BolsilloFelizDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("BolsilloFeliz")));


//builder.Services.AddControllers();
//builder.Services.AddOpenApi();

//var app = builder.Build();


//app.MapOpenApi();
//app.MapScalarApiReference();

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();
