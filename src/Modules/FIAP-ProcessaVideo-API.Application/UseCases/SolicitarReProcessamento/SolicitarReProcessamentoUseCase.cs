using FIAP_ProcessaVideo_API.Application.Abstractions;
using FIAP_ProcessaVideo_API.Common.Exceptions;
using FIAP_ProcessaVideo_API.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP_ProcessaVideo_API.Application.UseCases.SolicitarReProcessamento;

public class SolicitarReProcessamentoUseCase(IVideoUploadService videoUpload,
    IVideoRepository videoRepository,
    ISQSService sqsService) : IUseCase<string, bool>
{
    public async Task<bool> ExecuteAsync(string request)
    {
        if (string.IsNullOrWhiteSpace(request))
            throw new ApplicationNotificationException("Item da fila não informado.");
        
        var videoNoHistorico = await videoRepository.GetById(request);
        if (videoNoHistorico == null)
        {
            throw new ApplicationNotificationException("Vídeo não encontrado.");
        }

        Uri uri = new Uri(videoNoHistorico.Url);
        var videoExiste = await videoUpload.VideoExistsAsync(uri.AbsolutePath.TrimStart('/'));
        if (!videoExiste)
        {
            throw new ApplicationNotificationException("Vídeo não encontrado na nuvem.");
        }

        videoNoHistorico.AlterarStatus(Domain.Enums.StatusProcessamento.Aguardando);
        var recriado = await videoRepository.CreateAsync(videoNoHistorico);

        var sqsResponse = await sqsService.SendRequest(videoNoHistorico);

        return sqsResponse;
    }
}
