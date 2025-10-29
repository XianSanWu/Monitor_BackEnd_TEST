using Models.Common;

namespace Models.Dto.Requests
{
    public class MailHunterRequest
    {
        public class MailHunterSearchListRequest : BaseSearchModel
        {
            public MailHunterSearchListFieldModelRequest? FieldModel { get; set; }

        }

        public class MailHunterSearchListFieldModelRequest
        {
            public string? Department { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
        }
    }
}
