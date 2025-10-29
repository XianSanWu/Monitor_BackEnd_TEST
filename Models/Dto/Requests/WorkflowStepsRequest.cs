using Models.Common;

namespace Models.Dto.Requests
{
    public class WorkflowStepsRequest
    {

        public class WorkflowStepsSearchListRequest : BaseSearchModel
        {
            public WorkflowStepsSearchListFieldModelRequest? FieldModel { get; set; }
        }

        public class WorkflowStepsSearchListFieldModelRequest
        {
            public string? Channel { get; set; }
            public string? SendUuid { get; set; }
            public string? BatchId { get; set; }
            public string? SendUuidSort { get; set; }
        }

        public class WorkflowStepsKafkaRequest
        {
            public string? Channel { get; set; }
        }

    }
}
