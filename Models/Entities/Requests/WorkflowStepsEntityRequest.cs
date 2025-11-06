using Models.Common;

namespace Models.Entities.Requests
{
    public class WorkflowStepsEntityRequest
    {

        public class WorkflowStepsEntitySearchListRequest : BaseSearchModel
        {
            public WorkflowStepsEntitySearchListFieldModelRequest? FieldModel { get; set; }
        }

        public class WorkflowStepsEntitySearchListFieldModelRequest
        {
            public string? Channel { get; set; }
            public string? SendUuid { get; set; }
            public string? BatchId { get; set; }
            public string? SendUuidSort { get; set; }
        }

        public class WorkflowStepsEntityKafkaRequest
        {
            public string? Channel { get; set; }
        }

    }
}
