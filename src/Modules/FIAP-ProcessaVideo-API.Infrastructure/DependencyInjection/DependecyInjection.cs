using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.SQS;
using FIAP_ProcessaVideo_API.Application.Abstractions;
using FIAP_ProcessaVideo_API.Application.UseCases.ObterProcessamentoUsuario;
using FIAP_ProcessaVideo_API.Application.UseCases.SolicitarProcessamento;
using FIAP_ProcessaVideo_API.Application.UseCases.SolicitarReProcessamento;
using FIAP_ProcessaVideo_API.Domain.Abstractions;
using FIAP_ProcessaVideo_API.Infrastructure.Configurations;
using FIAP_ProcessaVideo_API.Infrastructure.Repositories.DynamoDb;
using FIAP_ProcessaVideo_API.Infrastructure.Services.S3;
using FIAP_ProcessaVideo_API.Infrastructure.Services.SQS;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace FIAP_ProcessaVideo_API.Infrastructure.DependencyInjection;

public static class DependecyInjection
{

    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        RegistrarContext(services, configuration);
        RegistrarServices(services, configuration);
        RegistrarAutenticacao(services, configuration);

        services.AddOptions();
        services.Configure<AwsSettings>(configuration.GetSection("AWS"));
    }

    private static void RegistrarContext(this IServiceCollection services, IConfiguration configuration)
    {
        var accessKeyId = configuration["AWS:AccessKeyId"];
        var secretAccessKey = configuration["AWS:SecretAccessKey"];

        services.Configure<DatabaseSettings>(options =>
        {
            options.TableName = configuration.GetSection("Database:TableName").Value;
        });

        services.Configure<S3Settings>(options =>
        {
            options.BucketName = configuration.GetSection("AWSS3:BucketName").Value;
        });

        services.Configure<SQSSettings>(options =>
        {
            options.QueueUrl = configuration.GetSection("SQS:QueueUrl").Value;
        });

        services.AddSingleton<IAmazonDynamoDB>(_ => new AmazonDynamoDBClient(
            new BasicAWSCredentials(accessKeyId, secretAccessKey),
            new AmazonDynamoDBConfig
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(configuration["AWS:Region"])
            }
        ));

        services.AddSingleton<IAmazonS3>(_ => new AmazonS3Client(
            new BasicAWSCredentials(accessKeyId, secretAccessKey),
            new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(configuration["AWS:Region"])
            }
         ));

        services.AddSingleton<IAmazonSQS>(_ => new AmazonSQSClient(
            new BasicAWSCredentials(accessKeyId, secretAccessKey),
            new AmazonSQSConfig
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(configuration["AWS:Region"])
            }
        ));
    }

    private static void RegistrarServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IVideoRepository, VideoRepository>();
        services.AddScoped<IVideoUploadService, VideoUploadService>();
        services.AddScoped<ISQSService, SQSService>();

        services.AddScoped<IUseCase<SolicitarProcessamentoRequest, bool>, SolicitarProcessamentoUseCase>();
        services.AddScoped<IUseCase<string, bool>, SolicitarReProcessamentoUseCase>();
        services.AddScoped<IUseCase<string, List<ObterProcessamentoUsuarioResponse>>, ObterProcessamentoUsuarioUseCase>();
    }

    private static void RegistrarAutenticacao(this IServiceCollection services, IConfiguration configuration)
    {
        string region = configuration["AWS:Region"]!;
        string cognitoAppClientId = configuration["AWS:Cognito:AppClientId"]!;
        string cognitoUserPoolId = configuration["AWS:Cognito:UserPoolId"]!;

        string authority = $"https://cognito-idp.{region}.amazonaws.com/{cognitoUserPoolId}";
        string validAudience = cognitoAppClientId;

        services.AddAuthorization();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = authority;
                    options.MapInboundClaims = false;
                    options.RequireHttpsMetadata = false;
                    options.IncludeErrorDetails = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateLifetime = true,
                        //ValidAudience = validAudience,
                        ValidateAudience = false,
                    };
                });
    }
}
