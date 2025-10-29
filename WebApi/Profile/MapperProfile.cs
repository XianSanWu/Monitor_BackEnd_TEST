using IdentityModel.OidcClient;
using Models.Dto.Requests;
using Models.Entities.Requests;
using Models.Entities.Responses;
using static Models.Dto.Requests.MailHunterRequest;
using static Models.Dto.Requests.PermissionRequest;
using static Models.Dto.Requests.UserRequest;
using static Models.Dto.Requests.WorkflowStepsRequest;
using static Models.Dto.Responses.AuditResponse.AuditSearchListResponse;
using static Models.Dto.Responses.MailHunterResponse;
using static Models.Dto.Responses.PermissionResponse;
using static Models.Dto.Responses.UserResponse;
using static Models.Dto.Responses.WorkflowStepsResponse.WorkflowStepsSearchListResponse;
using static Models.Entities.Requests.AuthEntityRequest;
using static Models.Entities.Requests.MailHunterEntityRequest;
using static Models.Entities.Requests.PermissionEntityRequest;
using static Models.Entities.Requests.UserEntityRequest;
using static Models.Entities.Requests.WorkflowStepsEntityRequest;

namespace WebAPi.Profile
{
    public class MapperProfile : AutoMapper.Profile
    {
        //.ReverseMap(); 不接受反向，故不使用
        public MapperProfile()
        {
            // Repository Entity → Service Response
            CreateMap<WorkflowEntity, WorkflowStepsSearchResponse>();
            CreateMap<ProjectMailCountEnyity, MailHunterSearchListDetailResponse>();
            CreateMap<FeaturePermissionEntity, PermissionSearchListResponse>();
            CreateMap<UserEntity, UserSearchListResponse>();
            CreateMap<AuditEntity, AuditSearchResponse>();

            // Service Request → Repository Request

            // Audit
            CreateMap<AuditSearchListRequest, AuditSearchListEntityRequest>();
            CreateMap<AuditSearchListFieldModelRequest, AuditSearchListFieldModelEntityRequest>();
            CreateMap<AuditRequest, AuditEntityRequest>();

            // Auth
            CreateMap<LoginRequest, LoginEntityRequest>();

            // User
            CreateMap<UserSearchListRequest, UserSearchListEntityRequest>();
            CreateMap<UserSearchListFieldModelRequest, UserSearchListFieldModelEntityRequest>();
            CreateMap<UserUpdateRequest, UserUpdateEntityRequest>();
            CreateMap<UserUpdateFieldRequest, UserUpdateFieldEntityRequest>();
            CreateMap<UserUpdateConditionRequest, UserUpdateConditionEntityRequest>();

            // Permission
            CreateMap<PermissionSearchListRequest, PermissionSearchListEntityRequest>();
            CreateMap<PermissionSearchListFieldModelRequest, PermissionSearchListFieldModelEntityRequest>();
            CreateMap<PermissionUpdateRequest, PermissionUpdateEntityRequest>();
            CreateMap<PermissionUpdateFieldRequest, PermissionUpdateFieldEntityRequest>();
            CreateMap<PermissionUpdateConditionRequest, PermissionUpdateConditionEntityRequest>();

            // WorkflowSteps
            CreateMap<WorkflowStepsSearchListRequest, WorkflowStepsSearchListEntityRequest>();
            CreateMap<WorkflowStepsSearchListFieldModelRequest, WorkflowStepsSearchListFieldModelEntityRequest>();
            CreateMap<WorkflowStepsKafkaRequest, WorkflowStepsKafkaEntityRequest>();

            // MailHunter
            CreateMap<MailHunterSearchListRequest, MailHunterSearchListEntityRequest>();
            CreateMap<MailHunterSearchListFieldModelRequest, MailHunterSearchListFieldModelEntityRequest>();

        }
    }
}
