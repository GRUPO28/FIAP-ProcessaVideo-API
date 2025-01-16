﻿using Amazon;
using Amazon.DynamoDBv2;
using Amazon.S3;
using FIAP_ProcessaVideo_API.Application.Abstractions;
using FIAP_ProcessaVideo_API.Application.UseCases.ObterProcessamentoUsuario;
using FIAP_ProcessaVideo_API.Application.UseCases.SolicitarProcessamento;
using FIAP_ProcessaVideo_API.Application.UseCases.SolicitarReProcessamento;
using FIAP_ProcessaVideo_API.Domain.Abstractions;
using FIAP_ProcessaVideo_API.Infrastructure.Repositories.DynamoDb;
using FIAP_ProcessaVideo_API.Infrastructure.Services.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FIAP_ProcessaVideo_API.Infrastructure.DependencyInjection;

public static class DependecyInjection
{

    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        RegistrarContext(services, configuration);
        RegistrarServices(services, configuration);
    }

    private static void RegistrarContext(this IServiceCollection services, IConfiguration configuration)
    {
        var teste = configuration.GetSection("Database:TableName").Value;
        var teste1 = configuration.GetSection("AWSS3:BucketName").Value;
        services.Configure<DatabaseSettings>(options =>
        {
            options.TableName = configuration.GetSection("Database:TableName").Value;
        });

        services.Configure<S3Settings>(options =>
        {
            options.BucketName = configuration.GetSection("AWSS3:BucketName").Value;
        });

        services.AddSingleton<IAmazonDynamoDB>(_ => new AmazonDynamoDBClient(RegionEndpoint.USEast1));
        services.AddSingleton<IAmazonS3>(_ => new AmazonS3Client(RegionEndpoint.USEast1));
    }

    private static void RegistrarServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IVideoRepository, VideoRepository>();
        services.AddScoped<IVideoUploadService, VideoUploadService>();

        services.AddScoped<IUseCase<SolicitarProcessamentoRequest, bool>, SolicitarProcessamentoUseCase>();
        services.AddScoped<IUseCase<string, bool>, SolicitarReProcessamentoUseCase>();
        services.AddScoped<IUseCase<string, List<ObterProcessamentoUsuarioResponse>>, ObterProcessamentoUsuarioUseCase>();
    }
}
