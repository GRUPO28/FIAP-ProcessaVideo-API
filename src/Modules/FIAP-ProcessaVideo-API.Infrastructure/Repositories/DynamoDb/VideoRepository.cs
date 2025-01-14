using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using FIAP_ProcessaVideo_API.Domain.Abstractions;
using FIAP_ProcessaVideo_API.Domain.Entities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FIAP_ProcessaVideo_API.Infrastructure.Repositories.DynamoDb;

public class VideoRepository(IAmazonDynamoDB dynamoDB, IOptions<DatabaseSettings> databaseSettings) : IVideoRepository
{
    private readonly IAmazonDynamoDB _dynamoDB = dynamoDB;
    private readonly IOptions<DatabaseSettings> _databaseSettings = databaseSettings;

    public async Task<bool> CreateAsync(Video video)
    {
        var videoAsJson = JsonSerializer.Serialize(video);
        var itemAsDocument = Document.FromJson(videoAsJson);
        var itemAsAttributes = itemAsDocument.ToAttributeMap();

        var createItemRequest = new PutItemRequest
        {

            TableName = _databaseSettings.Value.TableName,
            Item = itemAsAttributes
        };

        var response = await _dynamoDB.PutItemAsync(createItemRequest);

        return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
    }

    public async Task<List<Video>> GetByUser(string idUsuario)
    {
        var request = new QueryRequest
        {
            TableName = _databaseSettings.Value.TableName,
            IndexName = "IdUsuarioIndex", // Nome do seu índice secundário
            KeyConditionExpression = "Id_Usuario = :idUsuario",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                {":idUsuario", new AttributeValue { S = idUsuario }}
            }
        };

        var response = await _dynamoDB.QueryAsync(request);

        var videos = new List<Video>();

        foreach (var item in response.Items)
        {
            var document = Document.FromAttributeMap(item);
            var video = JsonSerializer.Deserialize<Video>(document.ToJson());

            if (video is not null)
                videos.Add(video);
        }

        return videos;
    }

    public async Task<Video> GetById(string id)
    {
        var request = new GetItemRequest
        {
            TableName = _databaseSettings.Value.TableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue { S = id }}
            }
        };

        var response = await _dynamoDB.GetItemAsync(request);

        if (response.Item is null || !response.Item.Any())
        {
            return null;
        }

        var document = Document.FromAttributeMap(response.Item);
        var video = JsonSerializer.Deserialize<Video>(document.ToJson());

        return video;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var deleteItemRequest = new DeleteItemRequest
        {
            TableName = _databaseSettings.Value.TableName,
            Key = new Dictionary<string, AttributeValue>
            {
                {"id", new AttributeValue { S = id } }
            }
        };

        var response = await _dynamoDB.DeleteItemAsync(deleteItemRequest);

        return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
    }
}
