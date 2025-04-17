using AutoMapper;
using Confluent.Kafka;
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
        IMapper mapper) : IWorkflowStepsService
    {
        private readonly ILogger<WorkflowStepsService> _logger = logger;

        public async Task<WorkflowStepsSearchListResponse> QueryWorkflowStepsSearchList(WorkflowStepsSearchListRequest searchReq, IConfiguration _config, CancellationToken cancellationToken = default)
        {
            #region 參數宣告
            //Task allTasks = null; 
            var result = new WorkflowStepsSearchListResponse();
            #endregion

            #region 流程
            var CDP_dbHelper = new DbHelper(_config, DBConnectionEnum.Cdp);
#if DEBUG
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

        public async Task<WorkflowStepsKafkaResponse> GetKafkaLag(WorkflowStepsKafkaRequest req, IConfiguration _config, CancellationToken cancellationToken = default)
        {

            #region 回傳範例
#if DEBUG
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
            }, cancellationToken);

        }


        private void ErrorHandler_Kafka(IConsumer<Ignore, string> consumer, Error error)
        {
            _logger.LogError($"Kafka Error發生：Fatal{error.IsFatal}, Error code：{error}");
        }


    }
}
