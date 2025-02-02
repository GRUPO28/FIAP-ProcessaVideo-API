using FIAP_ProcessaVideo_API.Domain.Entities;
using FIAP_ProcessaVideo_API.Domain.Enums;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace FIAP_ProcessaVideo_API_Tests.Steps.Domain;

[Binding]
public class VideoSteps
{
    private string _email = string.Empty;
    private string _url = string.Empty;
    private StatusProcessamento _status = StatusProcessamento.Falhou;
    private Exception _exception;
    
    [Given(@"a valid e-mail ""(.*)""")]
    public void GivenAValidEmail(string email)
    {
        _email = email;
    }
    [Given(@"a valid Url ""(.*)""")]
    public void GivenAValidUrl(string url)
    {
        _url = url;
    }
    [Given(@"a valid status ""(.*)""")]
    public void GivenAValidStatus(StatusProcessamento status)
    {
        _status = status;
    }
    
    [When(@"creating the entity")]
    public async Task WhenIRequestTheQueue()
    {
        try
        {
            var video = new Video(id: null ,email: _email, url: _url, status: _status);
        }
        catch (Exception ex)
        {
            _exception = ex;
        }
    }
    
    [Then(@"no exception should be thrown")]
    public void ThenNoExceptionShouldBeThrown()
    {
        _exception.Should().BeNull();
    }
    
    [Given(@"a invalid e-mail ""(.*)""")]
    public void GivenAInvalidEmail(string email)
    {
        _email = email;
    }
    [Given(@"a invalid Url ""(.*)""")]
    public void GivenAInvalidUrl(string url)
    {
        _url = url;
    }
    
    [Then(@"exception should be thrown")]
    public void ThenExceptionShouldBeThrown()
    {
        _exception.Should().NotBeNull();
    }
    
}