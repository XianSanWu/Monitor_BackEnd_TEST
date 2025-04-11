using Models.Dto.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }

        public class WorkflowStepsKafkaRequest
        {
            public string? Channel { get; set; }
        }

    }
}
