using Mapster;
using SGS.TaskTracker.Core.DTOs;
using SGS.TaskTracker.Core.Entities;

namespace SGS.TaskTracker.Application.Common.Mappings
{
    public static class MapsterConfig
    {
        public static void ConfigureMappings()
        {
            TypeAdapterConfig.GlobalSettings.Default.PreserveReference(true);

            TypeAdapterConfig<User, UserResponseDto>
                .NewConfig();

            TypeAdapterConfig<UserCreateDto, User>
                .NewConfig();

            TypeAdapterConfig<UserUpdateDto, User>
                .NewConfig()
                .IgnoreNullValues(true);

            TypeAdapterConfig<TaskItem, TaskResponseDto>
                .NewConfig()
                .Map(dest => dest.AssignedUserName,
                     src => src.AssignedUser != null ? src.AssignedUser.Username : null);

            TypeAdapterConfig<TaskCreateDto, TaskItem>
                .NewConfig();

            TypeAdapterConfig<TaskUpdateDto, TaskItem>
                .NewConfig()
                .IgnoreNullValues(true);
        }
    }
}
