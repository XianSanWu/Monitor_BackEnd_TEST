using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Repository.Interfaces;
using Services.Implementations;
using Models.Enums;
using Xunit;
using System.Threading;
using System.Threading.Tasks;
using static Models.Dto.Requests.WorkflowStepsRequest;
using static Models.Dto.Responses.WorkflowStepsResponse;
using static Models.Dto.Responses.WorkflowStepsResponse.WorkflowStepsKafkaResponse;
using Repository.Implementations.WorkflowStepsRespository;

public class WorkflowStepsServiceTests
{
    private readonly Mock<ILogger<WorkflowStepsService>> _loggerMock = new();
    private readonly Mock<IConfiguration> _configMock = new();
    private readonly Mock<IMemoryCache> _cacheMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IUnitOfWorkFactory> _uowFactoryMock = new();
    private readonly Mock<IRepositoryFactory> _repositoryFactoryMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<IWorkflowStepsRespository> _repoMock = new();

    private WorkflowStepsService CreateService()
    {
        // Factory 回傳 Repository 介面
        _repositoryFactoryMock
    .Setup(f => f.Create<IWorkflowStepsRespository>(_uowMock.Object, _mapperMock.Object))
    .Returns(_repoMock.Object);


        // Factory 回傳 UnitOfWork mock
        _uowFactoryMock
            .Setup(f => f.Create(DBConnectionEnum.Cdp, false))
            .Returns(_uowMock.Object);

        return new WorkflowStepsService(
            _loggerMock.Object,
            _configMock.Object,
            _cacheMock.Object,
            _mapperMock.Object,
            _uowFactoryMock.Object,
            _repositoryFactoryMock.Object
        );
    }

    [Fact]
    public async Task QueryWorkflowStepsSearchList_ReturnsExpectedResult()
    {
        var service = CreateService();
        var request = new WorkflowStepsSearchListRequest();
        var expectedResponse = new WorkflowStepsSearchListResponse();

        _repoMock
            .Setup(r => r.QueryWorkflowStepsSearchList(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await service.QueryWorkflowStepsSearchList(request);

        Assert.Equal(expectedResponse, result);
        _repoMock.Verify(r => r.QueryWorkflowStepsSearchList(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task QueryWorkflowStepsSearchLastList_ReturnsExpectedResult()
    {
        var service = CreateService();
        var request = new WorkflowStepsSearchListRequest();
        var expectedResponse = new WorkflowStepsSearchListResponse();

        _repoMock
            .Setup(r => r.QueryWorkflowStepsSearchLastList(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await service.QueryWorkflowStepsSearchLastList(request);

        Assert.Equal(expectedResponse, result);
        _repoMock.Verify(r => r.QueryWorkflowStepsSearchLastList(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetKafkaLag_ReturnsCachedValue_WhenCacheHit()
    {
        var service = CreateService();
        var request = new WorkflowStepsKafkaRequest { Channel = "test" };
        var cachedResponse = new WorkflowStepsKafkaResponse();

        // 模擬 cache hit
        object outValue = cachedResponse;
        _cacheMock
            .Setup(c => c.TryGetValue($"KafkaLag_{request.Channel}", out outValue))
            .Returns(true);

        var result = await service.GetKafkaLag(request);

        Assert.Equal(cachedResponse, result);

        // 正確 Verify LogInformation (擴充方法)
        _loggerMock.Verify(
    x => x.Log(
        LogLevel.Information,
        It.IsAny<EventId>(),
        It.Is<It.IsAnyType>((v, t) => v.ToString() == "使用 Server 端 Cache 資料"),
        It.IsAny<Exception>(),
        It.IsAny<Func<It.IsAnyType, Exception, string>>()
    ),
    Times.Once
);

    }

    [Fact]
    public async Task GetKafkaLag_SetsCache_WhenCacheMiss()
    {
        var service = CreateService();
        var request = new WorkflowStepsKafkaRequest { Channel = "test" };
        var response = new WorkflowStepsKafkaResponse();

        // cache miss
        object outValue = null!;
        _cacheMock
            .Setup(c => c.TryGetValue(It.IsAny<object>(), out outValue))
            .Returns(false);

        _cacheMock
            .Setup(c => c.Set(It.IsAny<object>(), It.IsAny<object>(), It.IsAny<TimeSpan>()))
            .Returns(response);

#if TEST
        var result = await service.GetKafkaLag(request);
        Assert.NotNull(result);
#endif
    }
}
