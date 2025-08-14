using Models.Entities;
using static Models.Dto.Responses.MailHunterResponse;
using static Models.Dto.Responses.PermissionResponse;
using static Models.Dto.Responses.UserResponse;
using static Models.Dto.Responses.WorkflowStepsResponse.WorkflowStepsSearchListResponse;

namespace WebAPi.Profile
{
    public class MapperProfile : AutoMapper.Profile
    {
        public MapperProfile()
        {
            CreateMap<WorkflowEntity, WorkflowStepsSearchResponse>();
            CreateMap<ProjectMailCountEnyity, MailHunterSearchListDetailResponse>();
            CreateMap<FeaturePermissionEntity, PermissionSearchListResponse>();
            CreateMap<UserEntity, UserSearchListResponse>();
            
        }
    }
}
