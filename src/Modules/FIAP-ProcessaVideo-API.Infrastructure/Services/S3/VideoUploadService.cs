using Amazon.S3.Transfer;
using Amazon.S3;
using Microsoft.Extensions.Options;
using FIAP_ProcessaVideo_API.Application.Abstractions;
using Amazon.S3.Model;
using FIAP_ProcessaVideo_API.Common.Exceptions;
using Amazon;
using Amazon.CognitoIdentity;
using FIAP_ProcessaVideo_API.Infrastructure.Configurations;
using FIAP_ProcessaVideo_API.Common.Abstractions;

namespace FIAP_ProcessaVideo_API.Infrastructure.Services.S3;

public class VideoUploadService : IVideoUploadService
{
    private readonly IHttpUserAccessor _httpUserAccessor;
    private readonly IAmazonS3 _amazonS3;
    private readonly AwsSettings _awsSettings;
    private readonly string _bucketName;

    private const string folder = "videos/";

    public VideoUploadService(IHttpUserAccessor httpUserAccessor, IAmazonS3 amazonS3, IOptions<S3Settings> s3Settings, IOptions<AwsSettings> awsSettings)
    {
        _httpUserAccessor = httpUserAccessor;
        _amazonS3 = amazonS3;
        _bucketName = s3Settings.Value.BucketName;
        _awsSettings = awsSettings.Value;
    }

    public async Task<string> UploadVideoAsync(Stream videoStream, string fileName, string contentType)
    {

        if (videoStream == null || videoStream.Length == 0)
        {
            throw new InfrastructureNotificationException("Nenhum arquivo de vídeo foi enviado.");
        }
  
        var keyName = $"{folder}{fileName}";

        try
        {
            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = videoStream,
                BucketName = _bucketName,
                Key = keyName,
                ContentType = contentType,
            };

            uploadRequest.Metadata.Add("uploadedBy", _httpUserAccessor.Email);

            // Acessa o serviço usando credenciais específicas do usuário
            var credentials = GetCognitoCredentials();

            using (var client = new AmazonS3Client(credentials))
            {
                var transferUtility = new TransferUtility(client);
                await transferUtility.UploadAsync(uploadRequest);
            }

            return $"https://{_bucketName}.s3.amazonaws.com/{keyName}";
        }
        catch (Exception ex)
        {
            throw new InfrastructureNotificationException("Erro ao fazer upload do vídeo.");
        }

    }

    public async Task<bool> VideoExistsAsync(string fileName)
    {
        try
        {
            var keyName = $"{folder}{fileName}";
            var request = new GetObjectMetadataRequest
            {
                BucketName = _bucketName,
                Key = fileName
            };

            var response = await _amazonS3.GetObjectMetadataAsync(request);

            if(response == null)
            {
                return false;
            }

            return true;
        }
        catch (Exception e)
        {

            throw new InfrastructureNotificationException("Erro ao verificar se o vídeo existe no S3");
        }
    }

    private CognitoAWSCredentials GetCognitoCredentials()
    {
        string providerName = $"cognito-idp.{_awsSettings.Region}.amazonaws.com/{_awsSettings.Cognito.UserPoolId}";

        // Exchange access token for AWS credentials
        CognitoAWSCredentials credentials = new CognitoAWSCredentials(_awsSettings.Cognito.IdentityPoolId, RegionEndpoint.USEast1);
        credentials.AddLogin(providerName, _httpUserAccessor.AuthorizationToken.Replace("Bearer ", ""));

        var t1 = credentials.GetCredentials().AccessKey;
        var t2 = credentials.GetCredentials().SecretKey;

        return credentials;
    }
}
