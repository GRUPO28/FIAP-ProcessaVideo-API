using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP_ProcessaVideo_API.Domain.Enums;

public enum StatusProcessamento
{
    Aguardando,
    Processando,
    Pronto,
    Falhou
}
