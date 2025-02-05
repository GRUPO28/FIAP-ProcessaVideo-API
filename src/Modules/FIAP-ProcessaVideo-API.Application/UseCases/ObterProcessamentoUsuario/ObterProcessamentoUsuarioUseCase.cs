using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
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
        public async Task<List<ObterProcessamentoUsuarioResponse>> ExecuteAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ApplicationNotificationException("Usuário não informado.");
            }

            var listaDeProcessamento = await videoRepository.GetByUser(email);

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
                    Identificador = processado.Id,
                    Status = processado.Status,
                    Video = uri.AbsolutePath.TrimStart('/').StartsWith("videos/") 
                        ? uri.AbsolutePath.TrimStart('/').Substring(7) 
                        : uri.AbsolutePath.TrimStart('/'),
                    Zip = processado.UrlZip
                });
            }
            
            return listaDeProcessamentoDto;
        }
    }
}
