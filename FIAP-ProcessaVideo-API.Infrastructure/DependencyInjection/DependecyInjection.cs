using Amazon;
using Amazon.DynamoDBv2;
using FIAP_ProcessaVideo_API.Domain.Abstractions;
using FIAP_ProcessaVideo_API.Infrastructure.Repositories;
using FIAP_ProcessaVideo_API.Infrastructure.Repositories.DynamoDb;
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
        services.Configure<DatabaseSettings>(options =>
        {
            options.TableName = configuration.GetSection("DatabaseSettings:TableName").Value;
            // Bind other properties of DatabaseSettings similarly
        });

        services.AddSingleton<IAmazonDynamoDB>(_ => new AmazonDynamoDBClient(RegionEndpoint.USEast1));
    }

    private static void RegistrarServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IVideoRepository, VideoRepository>();


    }
}
