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
    [JsonPropertyName("Url")]
    public string Url { get; private set; }

    [JsonPropertyName("Status")]
    public  StatusProcessamento Status { get; private set; }

    [JsonPropertyName("Email")]
    public string Email { get; private set; }


    public Video(string id, string url, StatusProcessamento status, string email) : base(id)
    {
        Url = url;
        Status = status;
        Email = email;
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

        if (!string.IsNullOrWhiteSpace(Email))
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
