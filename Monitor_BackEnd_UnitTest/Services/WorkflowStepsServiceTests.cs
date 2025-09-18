using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Repository.Interfaces;
using Services.Implementations;
using Models.Dto.Requests;
using Models.Dto.Responses;
using Models.Enums;
using Xunit;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Tests
{
    public class WorkflowStepsServiceTests
    {
        private readonly Mock<IUnitOfWorkFactory> _mockUowFactory;
        private readonly Mock<IRepositoryFactory> _mockRepoFactory;
        private readonly Mock<IWorkflowStepsRespository> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<WorkflowStepsService>> _mockLogger;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<IMemoryCache> _mockCache;

        private readonly WorkflowStepsService _service;

        public WorkflowStepsServiceTests()
        {
            _mockUowFactory = new Mock<IUnitOfWorkFactory>();
            _mockRepoFactory = new Mock<IRepositoryFactory>();
            _mockRepo = new Mock<IWorkflowStepsRespository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<WorkflowStepsService>>();
            _mockConfig = new Mock<IConfiguration>();
            _mockCache = new Mock<IMemoryCache>();

            // 設定 RepositoryFactory 回傳 Mock Interface
            _mockRepoFactory
                .Setup(f => f.Create<IWorkflowStepsRespository>(It.IsAny<object[]>()))
                .Returns(_mockRepo.Object);

            // 建立 Service
            _service = new WorkflowStepsService(
                _mockLogger.Object,
                _mockConfig.Object,
                _mockCache.Object,
                _mockMapper.Object,
                _mockUowFactory.Object,
                _mockRepoFactory.Object
            );
        }

        [Fact]
        public async Task QueryWorkflowStepsSearchLastList_ReturnsExpectedResult()
        {
            // Arrange
            var searchReq = new WorkflowStepsRequest.WorkflowStepsSearchListRequest
            {
                FieldModel = new WorkflowStepsRequest.WorkflowStepsSearchListFieldModelRequest
                {
                    Channel = "Email",
                    SendUuid = "123"
                },
            };

            var expectedResponse = new WorkflowStepsResponse.WorkflowStepsSearchListResponse
            {
                SearchItem = new List<WorkflowStepsResponse.WorkflowStepsSearchListResponse.WorkflowStepsSearchResponse>
                {
                    new WorkflowStepsResponse.WorkflowStepsSearchListResponse.WorkflowStepsSearchResponse
                    {
                        SN = 1,
                        SendUuid = "123",
                        Channel = "Email"
                    }
                },
                Page = searchReq.Page
            };

            // Mock UnitOfWork
            var mockUow = new Mock<IUnitOfWork>();
            _mockUowFactory
                .Setup(f => f.Create(It.IsAny<DBConnectionEnum>(), It.IsAny<bool>()))
                .Returns(mockUow.Object);

            // Mock Repository 方法
            _mockRepo
                .Setup(r => r.QueryWorkflowStepsSearchLastList(searchReq, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _service.QueryWorkflowStepsSearchLastList(searchReq);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.SearchItem);
            Assert.Equal("123", result.SearchItem[0].SendUuid);
            Assert.Equal("Email", result.SearchItem[0].Channel);

            // Verify Repository 被呼叫
            _mockRepo.Verify(r => r.QueryWorkflowStepsSearchLastList(searchReq, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
