using FIAP_ProcessaVideo_API.Common.Entities;
using FIAP_ProcessaVideo_API.Common.Exceptions;
using FIAP_ProcessaVideo_API.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FIAP_ProcessaVideo_API.Domain.Entities;

public class Video : Entity
{
    [JsonPropertyName("url")]
    public string Url { get; private set; }

    [JsonPropertyName("status")]
    public  StatusProcessamento Status { get; private set; }

    [JsonPropertyName("id_usuario")]
    public  string Id_Usuario { get; private set; }


    public Video(string id, string url, StatusProcessamento status, string idUsuario) : base(id)
    {
        Url = url;
        Status = status;
        Id_Usuario = idUsuario;
    }

    protected override void Validate()
    {
        if(!string.IsNullOrWhiteSpace(Url))
        {
            throw new DomainNotificationException("Url não foi informada.");
        }

        if(!Enum.IsDefined(typeof(StatusProcessamento), Status))
        {
            throw new DomainNotificationException("Status inválido.");
        }

        if (!string.IsNullOrWhiteSpace(Id_Usuario))
        {
            throw new DomainNotificationException("Id do usuário não informado.");
        }
    }

    public void AlterarStatus(StatusProcessamento status)
    {
        if(!Enum.IsDefined(typeof (StatusProcessamento), Status))
        {
            throw new DomainNotificationException("Status inválido.");
        }

        Status = status;
    }

}
