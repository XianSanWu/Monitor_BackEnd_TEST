using IdentityModel.OidcClient;
using static Models.Dto.Requests.AuditRequest;
using static Models.Dto.Requests.MailHunterRequest;
using static Models.Dto.Requests.PermissionRequest;
using static Models.Dto.Requests.UserRequest;
using static Models.Dto.Requests.WorkflowStepsRequest;
using static Models.Dto.Responses.AuditResponse;
using static Models.Dto.Responses.AuditResponse.AuditSearchListResponse;
using static Models.Dto.Responses.AuthResponse;
using static Models.Dto.Responses.MailHunterResponse;
using static Models.Dto.Responses.PermissionResponse.PermissionSearchListResponse;
using static Models.Dto.Responses.UserResponse;
using static Models.Dto.Responses.UserResponse.UserSearchListResponse;
using static Models.Dto.Responses.WorkflowStepsResponse;
using static Models.Dto.Responses.WorkflowStepsResponse.WorkflowStepsSearchListResponse;
using static Models.Entities.Requests.AuditEntityRequest;
using static Models.Entities.Requests.AuthEntityRequest;
using static Models.Entities.Requests.MailHunterEntityRequest;
using static Models.Entities.Requests.PermissionEntityRequest;
using static Models.Entities.Requests.UserEntityRequest;
using static Models.Entities.Requests.WorkflowStepsEntityRequest;
using static Models.Entities.Responses.AuditEntityResponse;
using static Models.Entities.Responses.FeaturePermissionEntityResponse;
using static Models.Entities.Responses.ProjectMailCountEntityResponse;
using static Models.Entities.Responses.UserEntityResponse;
using static Models.Entities.Responses.UserTokenEntityResponse;
using static Models.Entities.Responses.WorkflowEntityResponse;

namespace WebAPi.Profile
{
    public class MapperProfile : AutoMapper.Profile
    {
        //.ReverseMap(); 不接受反向，故不使用
        public MapperProfile()
        {
            #region Service Request → Repository Request
            // Service Request → Repository Request
            // Audit
            CreateMap<AuditSearchListRequest, AuditSearchListEntityRequest>();
            CreateMap<AuditSearchListFieldModelRequest, AuditSearchListFieldModelEntityRequest>();
            CreateMap<AuditCommomRequest, AuditEntityCommomRequest>();

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
            #endregion

            #region Repository Response → Service Response
            // Service Repository Response → Service Response
            // Audit
            // AuditEntity
            CreateMap<AuditEntitySearchListResponse, AuditSearchListResponse>()
                .ForMember(dest => dest.SearchItem, opt => opt.MapFrom(src => src.SearchItem));
            CreateMap<AuditEntity, AuditSearchResponse>();

            // Auth
            // UserEntity
            CreateMap<UserEntity, UserSearchResponse>();
            CreateMap<UserEntitySearchListResponse, UserSearchListResponse>()
                .ForMember(dest => dest.SearchItem, opt => opt.MapFrom(src => src.SearchItem));


            // PermissionEntity
            CreateMap<FeaturePermissionEntity, PermissionSearchResponse>();

            // MailHunter
            // ProjectMailCountEntity
            CreateMap<ProjectMailCountEntitySearchListResponse, MailHunterSearchListResponse>()
                .ForMember(dest => dest.SearchItem, opt => opt.MapFrom(src => src.SearchItem));
            CreateMap<ProjectMailCountEntity, MailHunterSearchListDetailResponse>();

            // Token
            // UserTokenEntity
            CreateMap<UserTokenEntity, AuthSearchResponse>();

            // WorkflowSteps
            // WorkflowStepsEntity
            CreateMap<WorkflowStepsEntitySearchListResponse, WorkflowStepsSearchListResponse>()
                .ForMember(dest => dest.SearchItem, opt => opt.MapFrom(src => src.SearchItem));
            CreateMap<WorkflowEntity, WorkflowStepsSearchResponse>();

            #endregion

        }
    }
}
