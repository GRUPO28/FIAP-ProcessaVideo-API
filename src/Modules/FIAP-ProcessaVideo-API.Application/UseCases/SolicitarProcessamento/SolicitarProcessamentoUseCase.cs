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
    private readonly IVideoRepository _videoRepository = videoRepository;
    private readonly IVideoUploadService _videoUploadService = videoUploadService;
    private readonly ISQSService _sqsService = sqsService;

    private static string[] validEntensions = [".mp4", ".mkv"];

    public async Task<bool> ExecuteAsync(SolicitarProcessamentoRequest request)
   {
        string fileExtension = Path.GetExtension(request.VideoFile.FileName);
        var teste = !validEntensions.Contains(fileExtension);
        if (string.IsNullOrEmpty(fileExtension) || !validEntensions.Contains(fileExtension))
        {
            throw new ApplicationNotificationException($"O arquivo enviado não possui uma extensão válida. Formatos aceitos: {string.Join(",", validEntensions)}");
        }

        var fileName = $"{Guid.NewGuid()}{fileExtension}";
        string videoUrl = "";

        using (var videoStream = request.VideoFile.OpenReadStream())
        {
            videoUrl = await _videoUploadService.UploadVideoAsync(videoStream, fileName, request.VideoFile.ContentType);
        }

        Video video = new Video(null, url:videoUrl, status: Domain.Enums.StatusProcessamento.Aguardando, email: httpUserAccessor.Email);

        var repositoryResponse = await _videoRepository.CreateAsync(video);

        var sqsResponse = await _sqsService.SendRequest(video);

        return sqsResponse;
   }
}
