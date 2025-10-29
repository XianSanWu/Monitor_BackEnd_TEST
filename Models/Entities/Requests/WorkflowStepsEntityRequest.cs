using Models.Common;

namespace Models.Entities.Requests
{
    public class WorkflowStepsEntityRequest
    {

        public class WorkflowStepsSearchListEntityRequest : BaseSearchModel
        {
            public WorkflowStepsSearchListFieldModelEntityRequest? FieldModel { get; set; }
        }

        public class WorkflowStepsSearchListFieldModelEntityRequest
        {
            public string? Channel { get; set; }
            public string? SendUuid { get; set; }
            public string? BatchId { get; set; }
            public string? SendUuidSort { get; set; }
        }

        public class WorkflowStepsKafkaEntityRequest
        {
            public string? Channel { get; set; }
        }

    }
}
