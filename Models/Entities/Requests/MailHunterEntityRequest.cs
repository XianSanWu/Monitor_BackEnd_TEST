using Models.Common;

namespace Models.Entities.Requests
{
    public class MailHunterEntityRequest
    {
        public class MailHunterEntitySearchListRequest : BaseSearchModel
        {
            public MailHunterEntitySearchListFieldModelRequest? FieldModel { get; set; }

        }

        public class MailHunterEntitySearchListFieldModelRequest
        {
            public string? Department { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
        }
    }
}
