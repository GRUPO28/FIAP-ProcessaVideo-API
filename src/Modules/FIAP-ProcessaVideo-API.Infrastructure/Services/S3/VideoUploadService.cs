using Amazon.S3.Transfer;
using Amazon.S3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using FIAP_ProcessaVideo_API.Application.Abstractions;
using Amazon.S3.Model;
using FIAP_ProcessaVideo_API.Common.Exceptions;

namespace FIAP_ProcessaVideo_API.Infrastructure.Services.S3;

public class VideoUploadService : IVideoUploadService
{
    private readonly IAmazonS3 _amazonS3;
    private readonly string _bucketName;

    public VideoUploadService(IAmazonS3 amazonS3, IOptions<S3Settings> s3Settings)
    {
        _amazonS3 = amazonS3;
        _bucketName = s3Settings.Value.BucketName;
    }

    public async Task<string> UploadVideoAsync(Stream videoStream, string fileName)
    {

        if (videoStream == null || videoStream.Length == 0)
        {
            throw new InfrastructureNotificationException("Nenhum arquivo de vídeo foi enviado.");
        }
  
        var keyName = $"{Guid.NewGuid()}_{fileName}";

        try
        {
            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = videoStream,
                BucketName = _bucketName,
                Key = keyName,
                ContentType = "video/mp4"
            };

            var transferUtility = new TransferUtility(_amazonS3);
            await transferUtility.UploadAsync(uploadRequest);

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
}
