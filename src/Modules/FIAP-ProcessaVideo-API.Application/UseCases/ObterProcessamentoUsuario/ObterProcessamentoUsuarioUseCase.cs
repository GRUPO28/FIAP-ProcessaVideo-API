﻿using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using FIAP_ProcessaVideo_API.Application.Abstractions;
using FIAP_ProcessaVideo_API.Common.Exceptions;
using FIAP_ProcessaVideo_API.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP_ProcessaVideo_API.Application.UseCases.ObterProcessamentoUsuario
{
    public class ObterProcessamentoUsuarioUseCase(IVideoRepository videoRepository) : IUseCase<string, List<ObterProcessamentoUsuarioResponse>>
    {
        private readonly IVideoRepository _videoRepository = videoRepository;

        public async Task<List<ObterProcessamentoUsuarioResponse>> ExecuteAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ApplicationNotificationException("Usuário não informado.");
            }

            var listaDeProcessamento = await _videoRepository.GetByUser(email);

            if(listaDeProcessamento == null || listaDeProcessamento.Count == 0)
            {
                throw new ApplicationNotificationException("Nenhum dado encontrado para o usuário informado.");
            }

            var listaDeProcessamentoDto = new List<ObterProcessamentoUsuarioResponse>();

            foreach(var processado in listaDeProcessamento)
            {
                Uri uri = new Uri(processado.Url);

                listaDeProcessamentoDto.Add(new ObterProcessamentoUsuarioResponse
                {
                    Email = processado.Email,
                    Status = processado.Status,
                    Video = uri.AbsolutePath.TrimStart('/')
                });
            }
            
            return listaDeProcessamentoDto;
        }
    }
}
