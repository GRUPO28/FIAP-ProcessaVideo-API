using FIAP_ProcessaVideo_API.Application.Abstractions;
using FIAP_ProcessaVideo_API.Application.UseCases.ObterProcessamentoUsuario;
using FIAP_ProcessaVideo_API.Application.UseCases.SolicitarProcessamento;
using FIAP_ProcessaVideo_API.Common.Abstractions;
using FIAP_ProcessaVideo_API.Controllers.ProcessamentoVideoControllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FIAP_ProcessaVideo_API_Tests.Controllers;

public class ProcessarVideoControllerTests
{
    private readonly Mock<IHttpUserAccessor> _httpUserAccessorMock;
    private readonly Mock<IUseCase<SolicitarProcessamentoRequest, bool>> _processamentoRequestMock;
    private readonly Mock<IUseCase<string, bool>> _reProcessamentoRequestMock;
    private readonly Mock<IUseCase<string, List<ObterProcessamentoUsuarioResponse>>> _obterProcessamentoMock;
    private readonly ProcessarVideoController _controller;

    public ProcessarVideoControllerTests()
    {
        _httpUserAccessorMock = new Mock<IHttpUserAccessor>();
        _processamentoRequestMock = new Mock<IUseCase<SolicitarProcessamentoRequest, bool>>();
        _reProcessamentoRequestMock = new Mock<IUseCase<string, bool>>();
        _obterProcessamentoMock = new Mock<IUseCase<string, List<ObterProcessamentoUsuarioResponse>>>();
        
        _controller = new ProcessarVideoController(
            _httpUserAccessorMock.Object,
            _processamentoRequestMock.Object,
            _reProcessamentoRequestMock.Object,
            _obterProcessamentoMock.Object);
    }

    [Fact]
    public async Task Processar_ShouldReturnBadRequest_WhenVideoFileIsTooLarge()
    {
        // Arrange
        var request = new SolicitarProcessamentoRequest
        {
            VideoFile = new FormFile(new MemoryStream(new byte[300 * 1024 * 1024]), 0, 300 * 1024 * 1024, "file", "video.mp4")
        };

        // Act
        var result = await _controller.Processar(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

        // Verificando se a mensagem de erro é a esperada
        Assert.Contains("Vídeo maior do que o permitido. Máximo 28 MB", badRequestResult.Value?.ToString());
    }
    
    [Fact]
    public async Task ReProcessar_ShouldReturnBadRequest_WhenProcessFails()
    {
        // Arrange
        var identificador = "some-identificador";
        _reProcessamentoRequestMock
            .Setup(x => x.ExecuteAsync(identificador))
            .ReturnsAsync(false); // Simula falha no reprocessamento

        // Act
        var result = await _controller.ReProcessar(identificador);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestResult>(result);
    }
    
    [Fact]
    public async Task ReProcessar_ShouldReturnOk_WhenReprocessSucceeds()
    {
        // Arrange
        var identificador = "identificador";
        _reProcessamentoRequestMock
            .Setup(x => x.ExecuteAsync(identificador))
            .ReturnsAsync(true); // Simula sucesso no reprocessamento

        // Act
        var result = await _controller.ReProcessar(identificador);

        // Assert
        var okResult = Assert.IsType<OkResult>(result);
    }
    
    [Fact]
    public async Task ObterFilaUsuario_ShouldReturnNoContent_WhenNoProcessesAreFound()
    {
        // Arrange
        _httpUserAccessorMock.Setup(x => x.Email).Returns("test@fiap.com");
        _obterProcessamentoMock
            .Setup(x => x.ExecuteAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<ObterProcessamentoUsuarioResponse>()); // Simula que não há processos

        // Act
        var result = await _controller.ObterFilaUsuario();

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task ObterFilaUsuario_ShouldReturnOk_WhenProcessesAreFound()
    {
        // Arrange
        _httpUserAccessorMock.Setup(x => x.Email).Returns("test@fiap.com");
        var mockProcess = new List<ObterProcessamentoUsuarioResponse>
        {
            new ObterProcessamentoUsuarioResponse { }
        };
        _obterProcessamentoMock
            .Setup(x => x.ExecuteAsync(It.IsAny<string>()))
            .ReturnsAsync(mockProcess); // Simula processos encontrados

        // Act
        var result = await _controller.ObterFilaUsuario();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsAssignableFrom<List<ObterProcessamentoUsuarioResponse>>(okResult.Value);
        Assert.NotEmpty(response); // Verifica se a lista não está vazia
    }

}