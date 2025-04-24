using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dto.Requests
{
    public class MsmqQueueRequest
    {
        public class MsmqQueueInfoRequest
        {
            public string? QueueName { get; set; }
        }
    }
}
