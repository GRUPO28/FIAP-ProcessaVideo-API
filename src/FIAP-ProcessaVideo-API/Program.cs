using FIAP_ProcessaVideo_API.Infrastructure.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Adicionando suporte ao CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

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

    // Configuração para suportar o envio de arquivos (multipart/form-data)
    c.MapType<IFormFile>(() => new OpenApiSchema { Type = "string", Format = "binary" });
});

//Mudeles
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Usando a política CORS
app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
