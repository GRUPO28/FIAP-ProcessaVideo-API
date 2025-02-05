using FIAP_ProcessaVideo_API.Application.Abstractions;
using FIAP_ProcessaVideo_API.Common.Abstractions;
using FIAP_ProcessaVideo_API.Common.Exceptions;
using FIAP_ProcessaVideo_API.Domain.Abstractions;
using FIAP_ProcessaVideo_API.Domain.Entities;

namespace FIAP_ProcessaVideo_API.Application.UseCases.SolicitarProcessamento;

public class SolicitarProcessamentoUseCase(
    IHttpUserAccessor httpUserAccessor,
    IVideoRepository videoRepository,
    IVideoUploadService videoUploadService,
    ISQSService sqsService) : IUseCase<SolicitarProcessamentoRequest, bool>
{
    private static string[] validEntensions = [".mp4", ".mkv"];

    public async Task<bool> ExecuteAsync(SolicitarProcessamentoRequest request)
   {
        if (request.VideoFile == null)
        { 
            throw new ApplicationNotificationException("O arquivo não foi informado.");
        }
       
        string fileExtension = Path.GetExtension(request.VideoFile.FileName);
        string originalFileName = Path.GetFileNameWithoutExtension(request.VideoFile.FileName);
        string safeFileName = originalFileName.Length > 20 ? originalFileName[..20] : originalFileName;
        
        if (string.IsNullOrEmpty(fileExtension) || !validEntensions.Contains(fileExtension))
        {
            throw new ApplicationNotificationException($"O arquivo enviado não possui uma extensão válida. Formatos aceitos: {string.Join(",", validEntensions)}");
        }
        var fileName = $"{Guid.NewGuid().ToString("N")[..5]}-{safeFileName}{fileExtension}";
        string videoUrl = "";

        using (var videoStream = request.VideoFile.OpenReadStream())
        {
            videoUrl = await videoUploadService.UploadVideoAsync(videoStream, fileName, request.VideoFile.ContentType);
        }

        Video video = new Video(null, url:videoUrl, status: Domain.Enums.StatusProcessamento.Aguardando, email: httpUserAccessor.Email);

        var repositoryResponse = await videoRepository.CreateAsync(video);

        var sqsResponse = await sqsService.SendRequest(video);

        return sqsResponse;
   }
}
