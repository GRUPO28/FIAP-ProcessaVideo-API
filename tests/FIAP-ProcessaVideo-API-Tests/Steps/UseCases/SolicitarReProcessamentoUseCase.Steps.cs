using FIAP_ProcessaVideo_API.Application.UseCases.SolicitarReProcessamento;
using FIAP_ProcessaVideo_API.Common.Exceptions;
using FIAP_ProcessaVideo_API.Domain.Entities;
using FIAP_ProcessaVideo_API.Domain.Enums;
using Moq;
using System;
using System.Threading.Tasks;
using FIAP_ProcessaVideo_API.Application.Abstractions;
using FIAP_ProcessaVideo_API.Domain.Abstractions;
using TechTalk.SpecFlow;

namespace FIAP_ProcessaVideo_API_Tests.Steps.UseCases
{
    [Binding]
    public class SolicitarReProcessamentoSteps
    {
        private readonly SolicitarReProcessamentoUseCase _useCase;
        private readonly Mock<IVideoRepository> _videoRepositoryMock;
        private readonly Mock<IVideoUploadService> _videoUploadMock;
        private readonly Mock<ISQSService> _sqsServiceMock;
        private bool _result;
        private Exception _exception;

        public SolicitarReProcessamentoSteps()
        {
            _videoRepositoryMock = new Mock<IVideoRepository>();
            _videoUploadMock = new Mock<IVideoUploadService>();
            _sqsServiceMock = new Mock<ISQSService>();
            _useCase = new SolicitarReProcessamentoUseCase(_videoUploadMock.Object, _videoRepositoryMock.Object, _sqsServiceMock.Object);
        }
        
        [Given(@"a video with ID ""(.*)"" exists in the repository")]
        public void GivenAVideoWithIdExistsInTheRepository(string id)
        {
            var video = new Video(id, "http://example.com/video.mp4", StatusProcessamento.Pronto, "user@example.com");
            _videoRepositoryMock.Setup(repo => repo.GetById(id)).ReturnsAsync(video);
        }

        [Given(@"the video with ID ""(.*)"" exists in S3")]
        public void GivenTheVideoWithIdExistsInS3(string id)
        {
            _videoUploadMock.Setup(service => service.VideoExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
        }

        [When(@"I request reprocessing for the video with ID ""(.*)""")]
        public async Task WhenIRequestReprocessingForTheVideoWithId(string id)
        {
            _result = await _useCase.ExecuteAsync(id);
        }
        
        [When(@"I request reprocessing for the video that does not exist in the repository with ID ""(.*)""")]
        public async Task WhenIRequestReprocessingForTheVideoThatDoesNotExistInTheRepository(string id)
        {
            _exception = await Record.ExceptionAsync(() => _useCase.ExecuteAsync(id));
        }

        [Then(@"the video should be recreated in the repository")]
        public void ThenTheVideoShouldBeRecreatedInTheRepository()
        {
            _videoRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Video>()), Times.Once);
        }

        [Given(@"a message should be sent to the SQS queue")]
        public void GivenAMessageShouldBeSentToTheSQSQueue()
        {
            _sqsServiceMock.Setup(x => x.SendRequest(It.IsAny<Video>())).ReturnsAsync(true);
        }

        [Then(@"the result should be true")]
        public void ThenTheResultShouldBeTrue()
        {
            Assert.True(_result);
        }
        
        [Given(@"a video with ID ""(.*)"" does not exist in the repository")]
        public void GivenAVideoWithIdDoesNotExistInTheRepository(string id)
        {
            _videoRepositoryMock.Setup(repo => repo.GetById(id)).ReturnsAsync((Video)null);
        }
        
        [When(@"I request reprocessing for the video that does not exist in S3 with ID ""(.*)""")]
        public async Task WhenIRequestReprocessingForTheVideoThatDoesNotExistInS3(string id)
        {
            _exception = await Record.ExceptionAsync(() => _useCase.ExecuteAsync(id));
        }
        
        [Given(@"the video with ID ""(.*)"" does not exist in S3")]
        public void GivenTheVideoWithIdDoesNotExistInS3(string id)
        {
            _videoUploadMock.Setup(service => service.VideoExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        }
        
        [Given(@"the video ID is not provided")]
        public void GivenTheVideoIdIsNotProvided()
        {
        }

        [When(@"I request reprocessing without providing the video ID")]
        public async Task WhenIRequestReprocessingWithoutProvidingTheVideoId()
        {
            _exception = await Record.ExceptionAsync(() => _useCase.ExecuteAsync(null));
        }

        [Then(@"an exception of type ApplicationNotificationException should be thrown with the message ""(.*)""")]
        public void ThenAnExceptionOfTypeApplicationNotificationExceptionShouldBeThrownWithTheMessageForNullId(string message)
        {
            Assert.IsType<ApplicationNotificationException>(_exception);
            Assert.Equal(message, _exception.Message);
            _videoRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Video>()), Times.Never);
            _sqsServiceMock.Verify(service => service.SendRequest(It.IsAny<Video>()), Times.Never);
        }
    }
}
