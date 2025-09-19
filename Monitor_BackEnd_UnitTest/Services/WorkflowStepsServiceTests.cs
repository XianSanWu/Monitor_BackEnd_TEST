using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Repository.Interfaces;
using Services.Implementations;
using Models.Dto.Responses;
using Models.Dto.Requests;

namespace Monitor_BackEnd_UnitTest.Services
{
    public class WorkflowStepsServiceTests
    {
        private readonly Mock<IUnitOfWorkFactory> _uowFactoryMock = new();
        private readonly Mock<IRepositoryFactory> _repositoryFactoryMock = new();
        private readonly Mock<IUnitOfWorkScopeAccessor> _scopeAccessorMock = new();
        private readonly Mock<IWorkflowStepsRespository> _repoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<IMemoryCache> _cacheMock = new();
        private readonly Mock<IConfiguration> _configMock = new();
        private readonly Mock<ILogger<WorkflowStepsService>> _loggerMock = new();

        private readonly WorkflowStepsService _service;

        public WorkflowStepsServiceTests()
        {
            // mock UoW
            var uowMock = new Mock<IUnitOfWork>();
            _scopeAccessorMock.SetupProperty(x => x.Current, uowMock.Object);

            // mock repository factory
            _repositoryFactoryMock
                .Setup(f => f.Create<IWorkflowStepsRespository>(It.IsAny<IUnitOfWorkScopeAccessor>()))
                .Returns(_repoMock.Object);

            _service = new WorkflowStepsService(
                _loggerMock.Object,
                _configMock.Object,
                _cacheMock.Object,
                _mapperMock.Object,
                _uowFactoryMock.Object,
                _repositoryFactoryMock.Object,
                _scopeAccessorMock.Object
            );
        }

        [Fact]
        public async Task QueryWorkflowStepsSearchList_ReturnsExpectedData()
        {
            var request = new WorkflowStepsRequest.WorkflowStepsSearchListRequest
            {
                FieldModel = new WorkflowStepsRequest.WorkflowStepsSearchListFieldModelRequest
                {
                    Channel = "Email"
                }
            };

            var expectedResponse = new WorkflowStepsResponse.WorkflowStepsSearchListResponse
            {
                SearchItem = new List<WorkflowStepsResponse.WorkflowStepsSearchListResponse.WorkflowStepsSearchResponse>
                {
                    new() { SN = 1, WorkflowUuid = "UUID123", Channel = "Email" }
                }
            };

            _repoMock
                .Setup(r => r.QueryWorkflowStepsSearchList(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            var result = await _service.QueryWorkflowStepsSearchList(request, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Single(result.SearchItem);
            Assert.Equal("UUID123", result.SearchItem[0].WorkflowUuid);
            Assert.Equal("Email", result.SearchItem[0].Channel);
        }

        [Fact]
        public async Task QueryWorkflowStepsSearchLastList_ReturnsExpectedData()
        {
            var request = new WorkflowStepsRequest.WorkflowStepsSearchListRequest
            {
                FieldModel = new WorkflowStepsRequest.WorkflowStepsSearchListFieldModelRequest
                {
                    Channel = "SMS"
                }
            };

            var expectedResponse = new WorkflowStepsResponse.WorkflowStepsSearchListResponse
            {
                SearchItem = new List<WorkflowStepsResponse.WorkflowStepsSearchListResponse.WorkflowStepsSearchResponse>
                {
                    new() { SN = 2, WorkflowUuid = "UUID456", Channel = "SMS" }
                }
            };

            _repoMock
                .Setup(r => r.QueryWorkflowStepsSearchLastList(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            var result = await _service.QueryWorkflowStepsSearchLastList(request, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Single(result.SearchItem);
            Assert.Equal("UUID456", result.SearchItem[0].WorkflowUuid);
            Assert.Equal("SMS", result.SearchItem[0].Channel);
        }
    }
}
