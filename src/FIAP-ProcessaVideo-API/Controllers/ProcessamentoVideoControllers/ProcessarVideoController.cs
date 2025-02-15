﻿using FIAP_ProcessaVideo_API.Application.Abstractions;
using FIAP_ProcessaVideo_API.Application.UseCases.ObterProcessamentoUsuario;
using FIAP_ProcessaVideo_API.Application.UseCases.SolicitarProcessamento;
using FIAP_ProcessaVideo_API.Common.Abstractions;
using FIAP_ProcessaVideo_API.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIAP_ProcessaVideo_API.Controllers.ProcessamentoVideoControllers;

[Authorize]
[Route("api/processar")]
public class ProcessarVideoController(
    IHttpUserAccessor httpUserAccessor,
    IUseCase<SolicitarProcessamentoRequest, bool> processamentoRequest, 
    IUseCase<string, bool> reProcessamentoRequest,
    IUseCase<string, List<ObterProcessamentoUsuarioResponse>> obterProcessamento) : BaseController
{
    const long maxFileSize = 10 * 1024 * 1024;
    
    [HttpPost]
    public async Task<ActionResult> Processar([FromForm] SolicitarProcessamentoRequest request)
    {
        try
        {
            
            if (request.VideoFile.Length > maxFileSize)
            {
                return BadRequest("Vídeo maior do que o permitido. Máximo 10 MB");
            }
            bool response = await processamentoRequest.ExecuteAsync(request);

            if (!response)
            {
                return BadRequest();
            }

            return Ok();
        }
        catch (NotificationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{nameof(ProcessarVideoController)}[{Processar}] - Unexpected Error - [{ex.Message}]]");
            return BadRequest(new { message = "Ocorreu um erro inesperado." });
        }
    }

    [HttpPut("reprocessar")]
    public async Task<ActionResult> ReProcessar([FromQuery] string identificador)
    {
        try
        {
            bool response = await reProcessamentoRequest.ExecuteAsync(identificador);

            if (!response)
            {
                return BadRequest();
            }

            return Ok();
        }
        catch (NotificationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{nameof(ProcessarVideoController)}[{ReProcessar}] - Unexpected Error - [{ex.Message}]]");
            return BadRequest(new { message = "Ocorreu um erro inesperado." });
        }
    }

    [HttpGet("filaUsuario")]
    public async Task<ActionResult> ObterFilaUsuario()
    {
        try
        {
            var response = await obterProcessamento.ExecuteAsync(httpUserAccessor.Email);

            if(response is null || response.Count < 1)
            {
                return NoContent();
            }

            return Ok(response);
        }
        catch (NotificationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{nameof(ProcessarVideoController)}[{ObterFilaUsuario}] - Unexpected Error - [{ex.Message}]]");
            return BadRequest(new { message = "Ocorreu um erro inesperado." });
        }
    }
}
