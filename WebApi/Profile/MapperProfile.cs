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
            CreateMap<AuditSearchListRequest, AuditEntitySearchListRequest>();
            CreateMap<AuditSearchListFieldModelRequest, AuditEntitySearchListFieldModelRequest>();
            CreateMap<AuditCommomRequest, AuditEntityCommomRequest>();

            // Auth
            CreateMap<LoginRequest, LoginEntityRequest>();

            // User
            CreateMap<UserSearchListRequest, UserEntitySearchListRequest>();
            CreateMap<UserSearchListFieldModelRequest, UserEntitySearchListFieldModelRequest>();
            CreateMap<UserUpdateRequest, UserEntityUpdateRequest>();
            CreateMap<UserUpdateFieldRequest, UserEntityUpdateFieldRequest>();
            CreateMap<UserUpdateConditionRequest, UserEntityUpdateConditionRequest>();

            // Permission
            CreateMap<PermissionSearchListRequest, PermissionEntitySearchListRequest>();
            CreateMap<PermissionSearchListFieldModelRequest, PermissionEntitySearchListFieldModelRequest>();
            CreateMap<PermissionUpdateRequest, PermissionEntityUpdateRequest>();
            CreateMap<PermissionUpdateFieldRequest, PermissionEntityUpdateFieldRequest>();
            CreateMap<PermissionUpdateConditionRequest, PermissionEntityUpdateConditionRequest>();

            // WorkflowSteps
            CreateMap<WorkflowStepsSearchListRequest, WorkflowStepsEntitySearchListRequest>();
            CreateMap<WorkflowStepsSearchListFieldModelRequest, WorkflowStepsEntitySearchListFieldModelRequest>();
            CreateMap<WorkflowStepsKafkaRequest, WorkflowStepsEntityKafkaRequest>();

            // MailHunter
            CreateMap<MailHunterSearchListRequest, MailHunterEntitySearchListRequest>();
            CreateMap<MailHunterSearchListFieldModelRequest, MailHunterEntitySearchListFieldModelRequest>();
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
