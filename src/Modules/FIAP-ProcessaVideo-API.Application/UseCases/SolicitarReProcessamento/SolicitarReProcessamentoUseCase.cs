using FIAP_ProcessaVideo_API.Application.Abstractions;
using FIAP_ProcessaVideo_API.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP_ProcessaVideo_API.Application.UseCases.SolicitarReProcessamento;

public class SolicitarReProcessamentoUseCase(IVideoUploadService videoUpload, IVideoRepository videoRepository) : IUseCase<string, bool>
{
    private readonly IVideoUploadService _videoUpload = videoUpload;
    private readonly IVideoRepository _videoRepository = videoRepository;

    public async Task<bool> ExecuteAsync(string request)
    {
        var videoNoHistorico = await _videoRepository.GetById(request);

        if (videoNoHistorico == null)
        {
            throw new Exception("Video não encontrado");
        }

        Uri uri = new Uri(videoNoHistorico.Url);

        var videoExiste = await _videoUpload.VideoExistsAsync(uri.AbsolutePath.TrimStart('/'));

        if (!videoExiste)
        {
            throw new NotImplementedException();
        }
        
        throw new NotImplementedException();
    }
}
