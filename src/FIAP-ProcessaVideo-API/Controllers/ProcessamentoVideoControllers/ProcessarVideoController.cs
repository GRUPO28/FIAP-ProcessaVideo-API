using FIAP_ProcessaVideo_API.Application.Abstractions;
using FIAP_ProcessaVideo_API.Application.UseCases.ObterProcessamentoUsuario;
using FIAP_ProcessaVideo_API.Application.UseCases.SolicitarProcessamento;
using FIAP_ProcessaVideo_API.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIAP_ProcessaVideo_API.Controllers.ProcessamentoVideoControllers;

[Authorize]
[Route("api/processar")]
public class ProcessarVideoController(
    IUseCase<SolicitarProcessamentoRequest, bool> processamentoRequest, 
    IUseCase<string, bool> reProcessamentoRequest,
    IUseCase<string, List<ObterProcessamentoUsuarioResponse>> obterProcessamento) : BaseController
{
    private readonly IUseCase<SolicitarProcessamentoRequest, bool> _processamentoRequest = processamentoRequest;
    private readonly IUseCase<string, bool> _reProcessamentoRequest = reProcessamentoRequest;
    private readonly IUseCase<string, List<ObterProcessamentoUsuarioResponse>> _obterProcessamento = obterProcessamento;

    [HttpPost]
    public async Task<ActionResult> Processar([FromForm] SolicitarProcessamentoRequest request)
    {
        try
        {
            bool response = await _processamentoRequest.ExecuteAsync(request);

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
    public async Task<ActionResult> ReProcessar([FromQuery] string request)
    {
        try
        {
            bool response = await _reProcessamentoRequest.ExecuteAsync(request);

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
    public async Task<ActionResult> ObterFilaUsuario([FromQuery] string email)
    {
        try
        {
            var response = await _obterProcessamento.ExecuteAsync(email);

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
            Console.WriteLine($"[{nameof(ProcessarVideoController)}[{ReProcessar}] - Unexpected Error - [{ex.Message}]]");
            return BadRequest(new { message = "Ocorreu um erro inesperado." });
        }
    }
}
