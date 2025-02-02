using FIAP_ProcessaVideo_API.Application.Abstractions;
using FIAP_ProcessaVideo_API.Application.UseCases.ObterProcessamentoUsuario;
using FIAP_ProcessaVideo_API.Domain.Abstractions;
using FIAP_ProcessaVideo_API.Domain.Entities;
using FIAP_ProcessaVideo_API.Domain.Enums;
using FluentAssertions;
using Moq;
using TechTalk.SpecFlow;

namespace FIAP_ProcessaVideo_API_Tests.Steps.UseCases;

[Binding]
public class ObterProcessamentoUsuarioSteps
{
    private readonly Mock<IVideoRepository> _mockVideoRepository = new();
    private readonly IUseCase<string, List<ObterProcessamentoUsuarioResponse>>  _useCase;
    private string _email;
    private List<ObterProcessamentoUsuarioResponse> _response;
    private Exception _exception;

    public ObterProcessamentoUsuarioSteps()
    {
        _useCase = new ObterProcessamentoUsuarioUseCase(_mockVideoRepository.Object);
    }

    [Given(@"A valid user e-mail ""(.*)""")]
    public void GivenAValidUserEmail(string email)
    {
        _email = email;
        
        _mockVideoRepository.Setup(x => x.GetByUser(_email))
            .ReturnsAsync(new List<Video>
            {
                new Video("1", "http://example.com/video1.mp4", StatusProcessamento.Processando, _email),
                new Video("2", "http://example.com/video2.mp4", StatusProcessamento.Pronto, _email)
            });
    }
    
    [Given("User e-mail was not sent")]
    public void GivenUserEmailWasNotSent()
    {
        
        _mockVideoRepository.Setup(x => x.GetByUser(_email))
            .ReturnsAsync(new List<Video>
            {
                new Video("1", "http://example.com/video1.mp4", StatusProcessamento.Processando, "Teste@gmail.com"),
                new Video("2", "http://example.com/video2.mp4", StatusProcessamento.Pronto, "Teste@gmail.com")
            });
    }

    [When(@"I request the queue")]
    public async Task WhenIRequestTheQueue()
    {
        try
        {
            _response = await _useCase.ExecuteAsync(_email);
        }
        catch (Exception ex)
        {
            _exception = ex;
        }
    }

    [Then(@"I should see at least data")]
    public void ThenIShouldSeeAtLeastData()
    {
        _response.Should().NotBeNull();
        _response.Should().HaveCountGreaterThan(0);
    }
    
    [Given(@"A valid user e-mail whith no data in DB ""(.*)""")]
    public void GivenAValidUserEmailWhithNoDataInDB(string email)
    {
        _email = email;
        _mockVideoRepository.Setup(x => x.GetByUser(_email))
            .ReturnsAsync(new List<Video>());
    }
    
    [Then(@"I should see an exception ""(.*)""")]
    public void ThenIShouldSeeAnException(string exception)
    {
        _exception.Should().NotBeNull();
        _exception.Message.Should().Be(exception);
    }
    

}