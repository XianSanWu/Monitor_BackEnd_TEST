using Models.Common;

namespace Models.Entities.Requests
{
    public class MailHunterEntityRequest
    {
        public class MailHunterSearchListEntityRequest : BaseSearchModel
        {
            public MailHunterSearchListFieldModelEntityRequest? FieldModel { get; set; }

        }

        public class MailHunterSearchListFieldModelEntityRequest
        {
            public string? Department { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
        }
    }
}
