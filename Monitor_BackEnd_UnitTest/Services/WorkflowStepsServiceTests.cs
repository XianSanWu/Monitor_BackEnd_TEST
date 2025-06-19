using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using AutoMapper;
using Services.Implementations;
using Repository.Interfaces;
using Models.Dto.Common;
using Models.Entities;
using static Models.Dto.Requests.WorkflowStepsRequest;
using static Models.Dto.Responses.WorkflowStepsResponse.WorkflowStepsSearchListResponse;
using static Models.Dto.Responses.WorkflowStepsResponse;

namespace Monitor_BackEnd_UnitTest.Services
{
    public class WorkflowStepsServiceTests
    {
        private readonly Mock<ILogger<WorkflowStepsService>> _mockLogger;
        private readonly Mock<IWorkflowStepsRespository> _mockRepository;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<IMemoryCache> _mockCache;
        private readonly IMapper _mapper;

        public WorkflowStepsServiceTests()
        {
            _mockLogger = new Mock<ILogger<WorkflowStepsService>>();
            _mockRepository = new Mock<IWorkflowStepsRespository>();
            _mockConfig = new Mock<IConfiguration>();
            _mockCache = new Mock<IMemoryCache>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<WorkflowEntity, WorkflowStepsSearchResponse>();
            });
            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task QueryWorkflowStepsSearchLastList_ShouldReturnExpectedResult()
        {
            // Arrange
            var request = new WorkflowStepsSearchListRequest
            {
                Page = new PageBase { PageIndex = 1, PageSize = 10 }
            };

            var expectedResponse = new WorkflowStepsSearchListResponse
            {
                Page = new PageBase { PageIndex = 1, PageSize = 10, TotalCount = 1 },
                SearchItem = new List<WorkflowStepsSearchResponse>
                {
                    new WorkflowStepsSearchResponse
                    {
                        SN = 1,
                        ActivityName = "Test Activity"
                        // 其他欄位可加上需要比對的值
                    }
                }
            };

            _mockRepository
                .Setup(repo => repo.QueryWorkflowStepsSearchLastList(It.IsAny<WorkflowStepsSearchListRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            var service = new WorkflowStepsService(
                _mockLogger.Object,
                _mockRepository.Object,
                _mockConfig.Object,
                _mockCache.Object,
                _mapper
            );

            // Act
            var result = await service.QueryWorkflowStepsSearchLastList(request);

            // Assert
            result.Should().NotBeNull();
            result.Page.TotalCount.Should().Be(1);
            result.SearchItem.Should().HaveCount(1);
            result.SearchItem.First().ActivityName.Should().Be("Test Activity");

            _mockRepository.Verify(r => r.QueryWorkflowStepsSearchLastList(It.IsAny<WorkflowStepsSearchListRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
