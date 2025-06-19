using AutoMapper;
using Confluent.Kafka;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models.Enums;
using Repository.Implementations;
using Repository.Implementations.WorkflowStepsRespository;
using Repository.Interfaces;
using Services.Interfaces;
using static Models.Dto.Requests.WorkflowStepsRequest;
using static Models.Dto.Responses.WorkflowStepsResponse;
using static Models.Dto.Responses.WorkflowStepsResponse.WorkflowStepsKafkaResponse;

namespace Services.Implementations
{
    public class WorkflowStepsService(
     ILogger<WorkflowStepsService> logger,
     IWorkflowStepsRespository wfsRepository,
     IConfiguration config,
     IMemoryCache cache,
     IMapper mapper) : IWorkflowStepsService
    {
        private readonly ILogger<WorkflowStepsService> _logger = logger;
        private readonly IConfiguration _config = config;
        private readonly IMemoryCache _cache = cache;
        private readonly IWorkflowStepsRespository _wfsRepository = wfsRepository;

        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(10);

        /// <summary>
        /// 工作進度查詢(最後一筆)
        /// </summary>
        /// <param name="searchReq"></param>
        /// <param name="_config"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<WorkflowStepsSearchListResponse> QueryWorkflowStepsSearchLastList(WorkflowStepsSearchListRequest searchReq, CancellationToken cancellationToken = default)
        {
            #region 參數宣告
            var result = new WorkflowStepsSearchListResponse();
            #endregion

            #region 流程
            result = await _wfsRepository.QueryWorkflowStepsSearchLastList(searchReq, cancellationToken).ConfigureAwait(false);
            
            return result;
            #endregion
        }

        /// <summary>
        /// 工作進度查詢
        /// </summary>
        /// <param name="searchReq"></param>
        /// <param name="_config"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<WorkflowStepsSearchListResponse> QueryWorkflowStepsSearchList(WorkflowStepsSearchListRequest searchReq, CancellationToken cancellationToken = default)
        {
            #region 參數宣告
            //Task allTasks = null; 
            var result = new WorkflowStepsSearchListResponse();
            #endregion

            #region 流程
            var CDP_dbHelper = new DbHelper(_config, DBConnectionEnum.Cdp);
#if TEST
            CDP_dbHelper = new DbHelper(_config, DBConnectionEnum.DefaultConnection);
#endif
            using (IDbHelper dbHelper = CDP_dbHelper)
            {
                IWorkflowStepsRespository _wfsRp = new WorkflowStepsRespository(dbHelper.UnitOfWork, mapper);
                result = await _wfsRp.QueryWorkflowStepsSearchList(searchReq, cancellationToken).ConfigureAwait(false);
            }

            return result;
            #endregion
        }

        /// <summary>
        /// 取得卡夫卡工作量
        /// </summary>
        /// <param name="req"></param>
        /// <param name="_config"></param>
        /// <param name="cancellationToken"></param>
        public async Task<WorkflowStepsKafkaResponse> GetKafkaLag(WorkflowStepsKafkaRequest req, CancellationToken cancellationToken = default)
        {
            string cacheKey = $"KafkaLag_{req.Channel}";

            if (_cache.TryGetValue(cacheKey, out var value) && value is WorkflowStepsKafkaResponse cachedValue)
            {
                _logger.LogInformation("使用 Server 端 Cache 資料");
                return cachedValue;
            }

            var result = await GetKafkaLagFromKafka(req, cancellationToken);
            _cache.Set(cacheKey, result, _cacheDuration);
            return result;
        }

        private async Task<WorkflowStepsKafkaResponse> GetKafkaLagFromKafka(WorkflowStepsKafkaRequest req, CancellationToken cancellationToken = default)
        {

            #region 回傳範例
#if TEST
            return await Task.FromResult(new WorkflowStepsKafkaResponse
            {
                PartitionLags = new List<KafkaLagInfo>
                {
                    new KafkaLagInfo
                    {
                        Partition = 0,
                        CommittedOffset = 1012,
                        HighWatermark = 1020,
                        Lag = 8
                    },
                    new KafkaLagInfo
                    {
                        Partition = 1,
                        CommittedOffset = 2000,
                        HighWatermark = 2025,
                        Lag = 25
                    },
                    new KafkaLagInfo
                    {
                        Partition = 1,
                        CommittedOffset = 2000,
                        HighWatermark = 2025,
                        Lag = 25
                    },
                },
                TotalLag = 33
            });
#endif
            #endregion

            return await Task.Run(() =>
            {
                var result = new WorkflowStepsKafkaResponse();
                var _topic = _config.GetValue<string>("Kafka:Topic");
                var _bootstrapServers = _config.GetValue<string>("Kafka:BootstrapServers");
                var _maxPollIntervalMs = _config.GetValue<int>("Kafka:MaxPollIntervalMs");
                var _consumeTimeSpan = _config.GetValue<int>("Kafka:ConsumeTimeSpan");

                var consumerConfig = new ConsumerConfig
                {
                    BootstrapServers = _bootstrapServers,
                    GroupId = req.Channel,
                    AutoOffsetReset = AutoOffsetReset.Earliest,
                    EnableAutoCommit = false,
                    MaxPollIntervalMs = _maxPollIntervalMs,
                };

                using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig)
                    .SetErrorHandler(ErrorHandler_Kafka)
                    .Build();

                using var adminClient = new AdminClientBuilder(consumerConfig).Build();
                var metadata = adminClient.GetMetadata(_topic, TimeSpan.FromSeconds(_consumeTimeSpan));
                var topicMetadata = metadata.Topics.First(t => t.Topic == _topic);

                foreach (var partitionMetadata in topicMetadata.Partitions)
                {
                    var partition = new TopicPartition(_topic, partitionMetadata.PartitionId);
                    var committedOffsets = consumer.Committed(new List<TopicPartition> { partition }, TimeSpan.FromSeconds(_consumeTimeSpan));
                    var committedOffset = committedOffsets.First().Offset;

                    var endOffsets = consumer.QueryWatermarkOffsets(partition, TimeSpan.FromSeconds(_consumeTimeSpan));
                    var highWatermark = endOffsets.High;
                    var lag = highWatermark - committedOffset;

                    result.PartitionLags.Add(new KafkaLagInfo
                    {
                        Partition = partition.Partition,
                        CommittedOffset = committedOffset.Value,
                        HighWatermark = highWatermark,
                        Lag = lag
                    });

                    result.TotalLag += lag;
                }

                return result;
            }, cancellationToken).ConfigureAwait(false);
        }

        private void ErrorHandler_Kafka(IConsumer<Ignore, string> consumer, Error error)
        {
            _logger.LogError($"Kafka Error發生：Fatal{error.IsFatal}, Error code：{error}");
        }


    }
}
