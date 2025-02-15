using FIAP_ProcessaVideo_API.Application.Abstractions;
using FIAP_ProcessaVideo_API.Application.UseCases.SolicitarProcessamento;
using FIAP_ProcessaVideo_API.Common.Abstractions;
using FIAP_ProcessaVideo_API.Common.Exceptions;
using FIAP_ProcessaVideo_API.Domain.Abstractions;
using FIAP_ProcessaVideo_API.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using TechTalk.SpecFlow;

namespace FIAP_ProcessaVideo_API_Tests.Steps.UseCases;

[Binding]
public class SolicitarProcessamentoUseCaseSteps
{
    private readonly Mock<IVideoRepository> _mockVideoRepository = new();
    private readonly Mock<IVideoUploadService> _mockVideoUploadService = new();
    private readonly Mock<ISQSService> _mockSqsService = new();
    private readonly Mock<IHttpUserAccessor> _mockHttpUserAccessor = new();
    private IFormFile _videoFile;
    private IUseCase<SolicitarProcessamentoRequest, bool> _useCase;
    private Exception _exception;
    private bool _result;

    public SolicitarProcessamentoUseCaseSteps()
    {
        _useCase = new SolicitarProcessamentoUseCase(
            _mockHttpUserAccessor.Object,
            _mockVideoRepository.Object,
            _mockVideoUploadService.Object,
            _mockSqsService.Object
        );
    }

    [Given(@"I have a valid video file named ""(.*)""")]
    public void GivenIHaveAValidVideoFileNamed(string fileName)
    {
        var fileContent = new byte[] { 1, 2, 3 };
        var mockFormFile = new Mock<IFormFile>();

        mockFormFile.Setup(f => f.FileName).Returns(fileName);
        mockFormFile.Setup(f => f.Length).Returns(fileContent.Length);
        mockFormFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(fileContent));
        mockFormFile.Setup(f => f.ContentType).Returns("video/mp4");

        _videoFile = mockFormFile.Object;

        _mockHttpUserAccessor.Setup(accessor => accessor.Email).Returns("user@example.com");
        
        _mockVideoUploadService.Setup(x => x.UploadVideoAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("http://s3.bucket/video.mp4");

        _mockVideoRepository.Setup(x => x.CreateAsync(It.IsAny<Video>())).ReturnsAsync(true);
        _mockSqsService.Setup(x => x.SendRequest(It.IsAny<Video>())).ReturnsAsync(true);
    }

    [Given(@"I have an invalid video file named ""(.*)""")]
    public void GivenIHaveAnInvalidVideoFileNamed(string fileName)
    {
        var fileContent = new byte[] { 1, 2, 3 };
        var mockFormFile = new Mock<IFormFile>();

        mockFormFile.Setup(f => f.FileName).Returns(fileName);
        mockFormFile.Setup(f => f.Length).Returns(fileContent.Length);
        mockFormFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(fileContent));
        mockFormFile.Setup(f => f.ContentType).Returns("text/plain");

        _videoFile = mockFormFile.Object;
    }

    [Given(@"I do not provide a video file")]
    public void GivenIDoNotProvideAVideoFile()
    {
        _videoFile = null;
    }

    [Given(@"the upload service is unavailable")]
    public void GivenTheUploadServiceIsUnavailable()
    {
        _mockVideoUploadService.Setup(x => x.UploadVideoAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new InfrastructureNotificationException("Error uploading the video."));
    }

    [When(@"I request video processing")]
    public async Task WhenIRequestVideoProcessing()
    {
        try
        {
            _result = await _useCase.ExecuteAsync(new SolicitarProcessamentoRequest
            {
                VideoFile = _videoFile
            });
        }
        catch (Exception ex)
        {
            _exception = ex;
        }
    }

    [Then(@"the process should succeed")]
    public void ThenTheProcessShouldSucceed()
    {
        Assert.True(_result);
    }

    [Then(@"an error should occur with the message ""(.*)""")]
    public void ThenAnErrorShouldOccurWithTheMessage(string expectedMessage)
    {
        _exception.Should().NotBeNull();
        _exception.Message.Should().Be(expectedMessage);
    }
}