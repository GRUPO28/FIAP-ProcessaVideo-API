using FIAP_ProcessaVideo_API.Application.Abstractions;
using FIAP_ProcessaVideo_API.Application.UseCases.SolicitarProcessamento;
using FIAP_ProcessaVideo_API.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace FIAP_ProcessaVideo_API.Controllers.ProcessamentoVideoControllers;

[Route("api/processar")]
public class ProcessarVideoController(
    IUseCase<SolicitarProcessamentoRequest, bool> processamentoRequest, 
    IUseCase<string, bool> reProcessamentoRequest) : BaseController
{
    private readonly IUseCase<SolicitarProcessamentoRequest, bool> _processamentoRequest = processamentoRequest;
    private readonly IUseCase<string, bool> _reProcessamentoRequest = reProcessamentoRequest;


    [HttpPost]
    public async Task<ActionResult> Processar([FromBody] SolicitarProcessamentoRequest request)
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

    [HttpPut]
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
}
