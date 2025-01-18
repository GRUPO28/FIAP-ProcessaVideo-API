using Amazon.SQS;
using Amazon.SQS.Model;
using FIAP_ProcessaVideo_API.Common.Exceptions;
using FIAP_ProcessaVideo_API.Domain.Abstractions;
using FIAP_ProcessaVideo_API.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP_ProcessaVideo_API.Infrastructure.Services.SQS;

public class SQSService(IAmazonSQS amazonSQS) : ISQSService
{
    private readonly IAmazonSQS _amazonSQS = amazonSQS;

    public async Task<bool> SendRequest(Video video)
    {
        try
        {
            var sendMessageRequest = new SendMessageRequest
            {
                QueueUrl = "your_queue_url",
                MessageBody = "Item para fila",
                MessageAttributes = new Dictionary<string, MessageAttributeValue>
            {
                { "Id", new MessageAttributeValue { DataType = "String", StringValue = video.Id } },
                { "Usuario", new MessageAttributeValue { DataType = "String", StringValue = video.Id_Usuario} },
                { "Url", new MessageAttributeValue { DataType = "Number", StringValue = video.Url } },
            }
            };

            var response = await _amazonSQS.SendMessageAsync(sendMessageRequest);

            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }
        catch(Exception ex) 
        {
            throw new InfrastructureNotificationException("Problema no SQS.");
        }
       
    }
}
