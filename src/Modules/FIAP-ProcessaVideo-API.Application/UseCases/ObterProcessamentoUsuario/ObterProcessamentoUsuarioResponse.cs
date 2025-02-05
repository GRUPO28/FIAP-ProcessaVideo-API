using FIAP_ProcessaVideo_API.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FIAP_ProcessaVideo_API.Application.UseCases.ObterProcessamentoUsuario;

public class ObterProcessamentoUsuarioResponse
{
    public string Identificador { get; set; }
    public string Video { get; set; }
    public StatusProcessamento Status { get; set; }
    public string Zip { get; set; }
}
