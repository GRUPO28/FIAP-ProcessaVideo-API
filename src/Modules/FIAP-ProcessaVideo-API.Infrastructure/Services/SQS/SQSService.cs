using Amazon.SQS;
using Amazon.SQS.Model;
using FIAP_ProcessaVideo_API.Common.Exceptions;
using FIAP_ProcessaVideo_API.Domain.Abstractions;
using FIAP_ProcessaVideo_API.Domain.Entities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP_ProcessaVideo_API.Infrastructure.Services.SQS;

public class SQSService(IAmazonSQS amazonSQS, IOptions<SQSSettings> sqsOptions) : ISQSService
{
    public async Task<bool> SendRequest(Video video)
    {
        try
        {
            var sendMessageRequest = new SendMessageRequest
            {
                QueueUrl = sqsOptions.Value.QueueUrl,
                MessageBody = "Item para fila",
                MessageAttributes = new Dictionary<string, MessageAttributeValue>
            {
                { "Id", new MessageAttributeValue { DataType = "String", StringValue = video.Id } },
                { "Usuario", new MessageAttributeValue { DataType = "String", StringValue = video.Email} },
                { "Url", new MessageAttributeValue { DataType = "String", StringValue = video.Url } },
            }
            };

            var response = await amazonSQS.SendMessageAsync(sendMessageRequest);

            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }
        catch(Exception ex) 
        {
            throw new InfrastructureNotificationException("Erro inesperado ao enviar vídeo para a fila.");
        }
       
    }
}
