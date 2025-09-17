using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Services.Implementations;
using Repository.Interfaces;
using Models.Dto.Common;
using static Models.Dto.Requests.WorkflowStepsRequest;
using static Models.Dto.Responses.WorkflowStepsResponse;
using static Models.Dto.Responses.WorkflowStepsResponse.WorkflowStepsKafkaResponse;
using Models.Entities;
using static Models.Dto.Responses.WorkflowStepsResponse.WorkflowStepsSearchListResponse;

namespace Monitor_BackEnd_UnitTest.Services
{
    public class WorkflowStepsServiceTests
    {
        private readonly Mock<ILogger<WorkflowStepsService>> _mockLogger;
        private readonly Mock<IWorkflowStepsRespository> _mockRepository;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly MemoryCache _memoryCache;
        private readonly IMapper _mapper;
        private readonly Mock<IHostEnvironment> _mockEnv;

        public WorkflowStepsServiceTests()
        {
            _mockLogger = new Mock<ILogger<WorkflowStepsService>>();
            _mockRepository = new Mock<IWorkflowStepsRespository>();
            _mockConfig = new Mock<IConfiguration>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _mockEnv = new Mock<IHostEnvironment>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<WorkflowEntity, WorkflowStepsSearchResponse>();
            });
            _mapper = mapperConfig.CreateMapper();
        }

        [Fact]
        public async Task QueryWorkflowStepsSearchLastList_ShouldUseMockRepository_WhenEnvIsTest()
        {
            // Arrange
            _mockEnv.Setup(e => e.EnvironmentName).Returns("Test");

            var expected = new WorkflowStepsSearchListResponse
            {
                Page = new PageBase { PageIndex = 1, PageSize = 10, TotalCount = 1 },
                SearchItem = new List<WorkflowStepsSearchResponse>
                {
                    new WorkflowStepsSearchResponse { SN = 1, ActivityName = "UnitTest Activity" }
                }
            };

            _mockRepository.Setup(r =>
                r.QueryWorkflowStepsSearchLastList(It.IsAny<WorkflowStepsSearchListRequest>(), It.IsAny<CancellationToken>())
            ).ReturnsAsync(expected);

            var service = new WorkflowStepsService(
                _mockLogger.Object,
                _mockRepository.Object,
                _mockConfig.Object,
                _memoryCache,
                _mapper,
                _mockEnv.Object
            );

            var request = new WorkflowStepsSearchListRequest { Page = new PageBase { PageIndex = 1, PageSize = 10 } };

            // Act
            var result = await service.QueryWorkflowStepsSearchLastList(request);

            // Assert
            result.Should().NotBeNull();
            result.SearchItem.Should().ContainSingle();
            result.SearchItem.First().ActivityName.Should().Be("UnitTest Activity");
        }

        [Fact]
        public async Task QueryWorkflowStepsSearchList_ShouldUseMockRepository_WhenEnvIsTest()
        {
            // Arrange
            _mockEnv.Setup(e => e.EnvironmentName).Returns("Test");

            var expected = new WorkflowStepsSearchListResponse
            {
                Page = new PageBase { PageIndex = 1, PageSize = 5, TotalCount = 1 },
                SearchItem = new List<WorkflowStepsSearchResponse>
                {
                    new WorkflowStepsSearchResponse { SN = 2, ActivityName = "List Activity" }
                }
            };

            _mockRepository.Setup(r =>
                r.QueryWorkflowStepsSearchList(It.IsAny<WorkflowStepsSearchListRequest>(), It.IsAny<CancellationToken>())
            ).ReturnsAsync(expected);

            var service = new WorkflowStepsService(
                _mockLogger.Object,
                _mockRepository.Object,
                _mockConfig.Object,
                _memoryCache,
                _mapper,
                _mockEnv.Object
            );

            var request = new WorkflowStepsSearchListRequest { Page = new PageBase { PageIndex = 1, PageSize = 5 } };

            // Act
            var result = await service.QueryWorkflowStepsSearchList(request);

            // Assert
            result.Should().NotBeNull();
            result.Page.PageSize.Should().Be(5);
            result.SearchItem.Should().ContainSingle();
            result.SearchItem.First().ActivityName.Should().Be("List Activity");
        }

        [Fact]
        public async Task GetKafkaLag_ShouldReturnFromCache_WhenCacheExists()
        {
            // Arrange
            _mockEnv.Setup(e => e.EnvironmentName).Returns("Development");

            var req = new WorkflowStepsKafkaRequest { Channel = "ch1" };
            var expected = new WorkflowStepsKafkaResponse
            {
                PartitionLags = new List<KafkaLagInfo>
                {
                    new KafkaLagInfo { Partition = 0, CommittedOffset = 10, HighWatermark = 20, Lag = 10 }
                },
                TotalLag = 10
            };

            _memoryCache.Set($"KafkaLag_{req.Channel}", expected);

            var service = new WorkflowStepsService(
                _mockLogger.Object,
                _mockRepository.Object,
                _mockConfig.Object,
                _memoryCache,
                _mapper,
                _mockEnv.Object
            );

            // Act
            var result = await service.GetKafkaLag(req);

            // Assert
            result.Should().BeSameAs(expected);
        }

        [Fact]
        public async Task GetKafkaLag_ShouldCallKafkaMethod_WhenCacheIsEmpty()
        {
            // Arrange
            _mockEnv.Setup(e => e.EnvironmentName).Returns("Development");

            var req = new WorkflowStepsKafkaRequest { Channel = "ch2" };

            // 設定 config 模擬值
            _mockConfig.Setup(c => c.GetValue<string>("Kafka:Topic")).Returns("test-topic");
            _mockConfig.Setup(c => c.GetValue<string>("Kafka:BootstrapServers")).Returns("localhost:9092");
            _mockConfig.Setup(c => c.GetValue<int>("Kafka:MaxPollIntervalMs")).Returns(1000);
            _mockConfig.Setup(c => c.GetValue<int>("Kafka:ConsumeTimeSpan")).Returns(5);

            var service = new WorkflowStepsService(
                _mockLogger.Object,
                _mockRepository.Object,
                _mockConfig.Object,
                _memoryCache,
                _mapper,
                _mockEnv.Object
            );

            // 因為真的呼叫 Kafka 太重，這裡只驗證流程
            Func<Task> act = async () => await service.GetKafkaLag(req);

            // Assert
            await act.Should().NotThrowAsync(); // 確保不丟例外
        }
    }
}
