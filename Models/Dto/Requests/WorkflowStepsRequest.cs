using Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dto.Requests
{
    public class WorkflowStepsRequest
    {

        public class WorkflowStepsSearchListRequest : BaseSearchRequest
        {
            public int MyProperty { get; set; }
        }

    }
}
