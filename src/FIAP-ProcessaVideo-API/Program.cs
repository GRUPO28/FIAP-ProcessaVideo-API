using FIAP_ProcessaVideo_API.Common.Abstractions;
using FIAP_ProcessaVideo_API.Infrastructure.DependencyInjection;
using FIAP_ProcessaVideo_API.Middlewares;
using FIAP_ProcessaVideo_API.User;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration
       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
       .AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Minha API", Version = "v1" });

    // Configuracao para suportar o envio de arquivos (multipart/form-data)
    c.MapType<IFormFile>(() => new OpenApiSchema { Type = "string", Format = "binary" });
});

//Modules
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IHttpUserAccessor, HttpUserAccessor>();
builder.Services.AddSingleton<AuthenticationMiddleware>();
builder.Services.AddInfrastructure(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<AuthenticationMiddleware>();

app.MapControllers();

app.Run();
