using FIAP_ProcessaVideo_API.Application.Abstractions;
using FIAP_ProcessaVideo_API.Application.UseCases.SolicitarProcessamento;
using FIAP_ProcessaVideo_API.Common.Abstractions;
using FIAP_ProcessaVideo_API.Common.Exceptions;
using FIAP_ProcessaVideo_API.Domain.Abstractions;
using FIAP_ProcessaVideo_API.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;

namespace FIAP_ProcessaVideo_API_Tests.Steps.UseCases;

public class Teste
{
    [Fact]
    public async Task ExecuteAsync_ShouldReturnTrue_WhenProcessingIsSuccessful()
    {
        // Arrange
        var mockVideoRepository = new Mock<IVideoRepository>();
        var mockVideoUploadService = new Mock<IVideoUploadService>();
        var mockSqsService = new Mock<ISQSService>();
        var mockHttpUserAccessor = new Mock<IHttpUserAccessor>();

        // Criação de um mock de IFormFile (representando o arquivo de vídeo)
        var mockFormFile = new Mock<IFormFile>();
        var fileName = "video.mp4"; // Certifique-se de que o nome do arquivo tenha a extensão correta
        var fileContent = new byte[] { 1, 2, 3 }; // Conteúdo de exemplo do arquivo

        mockFormFile.Setup(f => f.FileName).Returns(fileName);
        mockFormFile.Setup(f => f.Length).Returns(fileContent.Length);
        mockFormFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(fileContent));
        mockFormFile.Setup(f => f.ContentType).Returns("video/mp4");

        mockVideoUploadService.Setup(x => x.UploadVideoAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("http://s3.bucket/video.mp4"); // Simula o upload do vídeo

        mockVideoRepository.Setup(x => x.CreateAsync(It.IsAny<Video>()))
            .ReturnsAsync(true); // Simula sucesso ao criar o vídeo no repositório

        mockSqsService.Setup(x => x.SendRequest(It.IsAny<Video>()))
            .ReturnsAsync(true); // Simula sucesso ao enviar o vídeo para a fila SQS

        var useCase = new SolicitarProcessamentoUseCase(
            mockHttpUserAccessor.Object,
            mockVideoRepository.Object,
            mockVideoUploadService.Object,
            mockSqsService.Object
        );

        // Act
        var result = await useCase.ExecuteAsync(new SolicitarProcessamentoRequest
        {
            VideoFile = mockFormFile.Object // Passa o mock do IFormFile
        });

        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public async Task ExecuteAsync_ShouldThrowException_WhenFileExtensionIsInvalid()
    {
        // Arrange
        var mockVideoRepository = new Mock<IVideoRepository>();
        var mockVideoUploadService = new Mock<IVideoUploadService>();
        var mockSqsService = new Mock<ISQSService>();
        var mockHttpUserAccessor = new Mock<IHttpUserAccessor>();

        // Criação de um mock de IFormFile (representando o arquivo de vídeo)
        var mockFormFile = new Mock<IFormFile>();
        var fileName = "video.txt"; // Extensão inválida
        var fileContent = new byte[] { 1, 2, 3 }; // Conteúdo de exemplo do arquivo

        mockFormFile.Setup(f => f.FileName).Returns(fileName);
        mockFormFile.Setup(f => f.Length).Returns(fileContent.Length);
        mockFormFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(fileContent));
        mockFormFile.Setup(f => f.ContentType).Returns("text/plain");

        var useCase = new SolicitarProcessamentoUseCase(
            mockHttpUserAccessor.Object,
            mockVideoRepository.Object,
            mockVideoUploadService.Object,
            mockSqsService.Object
        );

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApplicationNotificationException>(
            () => useCase.ExecuteAsync(new SolicitarProcessamentoRequest
            {
                VideoFile = mockFormFile.Object // Passa o mock do IFormFile
            })
        );

        exception.Message.Should().Be("O arquivo enviado não possui uma extensão válida. Formatos aceitos: .mp4,.mkv");
    }
    
    [Fact]
    public async Task ExecuteAsync_ShouldThrowException_WhenVideoFileIsNull()
    {
        // Arrange
        var mockVideoRepository = new Mock<IVideoRepository>();
        var mockVideoUploadService = new Mock<IVideoUploadService>();
        var mockSqsService = new Mock<ISQSService>();
        var mockHttpUserAccessor = new Mock<IHttpUserAccessor>();

        var useCase = new SolicitarProcessamentoUseCase(
            mockHttpUserAccessor.Object,
            mockVideoRepository.Object,
            mockVideoUploadService.Object,
            mockSqsService.Object
        );

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApplicationNotificationException>(
            () => useCase.ExecuteAsync(new SolicitarProcessamentoRequest
            {
                VideoFile = null // Arquivo nulo
            })
        );

        exception.Message.Should().Be("O arquivo não foi informado.");
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowException_WhenUploadFails()
    {
        // Arrange
        var mockVideoRepository = new Mock<IVideoRepository>();
        var mockVideoUploadService = new Mock<IVideoUploadService>();
        var mockSqsService = new Mock<ISQSService>();
        var mockHttpUserAccessor = new Mock<IHttpUserAccessor>();

        var mockFormFile = new Mock<IFormFile>();
        var fileName = "video.mp4";
        var fileContent = new byte[] { 1, 2, 3 };

        mockFormFile.Setup(f => f.FileName).Returns(fileName);
        mockFormFile.Setup(f => f.Length).Returns(fileContent.Length);
        mockFormFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(fileContent));
        mockFormFile.Setup(f => f.ContentType).Returns("video/mp4");

        mockVideoUploadService.Setup(x => x.UploadVideoAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new InfrastructureNotificationException("Erro ao fazer upload do vídeo."));

        var useCase = new SolicitarProcessamentoUseCase(
            mockHttpUserAccessor.Object,
            mockVideoRepository.Object,
            mockVideoUploadService.Object,
            mockSqsService.Object
        );

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InfrastructureNotificationException>(
            () => useCase.ExecuteAsync(new SolicitarProcessamentoRequest
            {
                VideoFile = mockFormFile.Object
            })
        );

        exception.Message.Should().Be("Erro ao fazer upload do vídeo.");
    }
}