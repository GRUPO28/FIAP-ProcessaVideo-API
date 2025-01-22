﻿using FIAP_ProcessaVideo_API.Application.Abstractions;
using FIAP_ProcessaVideo_API.Domain.Abstractions;
using FIAP_ProcessaVideo_API.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP_ProcessaVideo_API.Application.UseCases.SolicitarProcessamento;

public class SolicitarProcessamentoUseCase(IVideoRepository videoRepository,
    IVideoUploadService videoUploadService,
    ISQSService sqsService) : IUseCase<SolicitarProcessamentoRequest, bool>
{
    private readonly IVideoRepository _videoRepository = videoRepository;
    private readonly IVideoUploadService _videoUploadService = videoUploadService;
    private readonly ISQSService _sqsService = sqsService;

    public async Task<bool> ExecuteAsync(SolicitarProcessamentoRequest request)
   {
        var fileName = Guid.NewGuid().ToString();
        string videoUrl = "";

        using (var videoStream = request.VideoFile.OpenReadStream())
        {
            
            videoUrl = await _videoUploadService.UploadVideoAsync(videoStream, fileName);
        }

        Video video = new Video(null, url:videoUrl, status: Domain.Enums.StatusProcessamento.Aguardando, email: "teste1@gmail.com");

        var repositoryResponse = await _videoRepository.CreateAsync(video);

        var sqsResponse = await _sqsService.SendRequest(video);

        return sqsResponse;
   }
}
