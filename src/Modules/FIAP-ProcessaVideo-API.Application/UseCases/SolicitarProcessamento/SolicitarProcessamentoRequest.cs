using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP_ProcessaVideo_API.Application.UseCases.SolicitarProcessamento;

public class SolicitarProcessamentoRequest
{
    public required IFormFile VideoFile { get; set; }

}
