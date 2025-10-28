using Mapster;
using SGS.TaskTracker.Core.DTOs;
using SGS.TaskTracker.Core.Entities;
using SGS.TaskTracker.Dtos;

namespace SGS.TaskTracker.Application.Common.Mappings
{
    public static class MappingExtensions
    {
        public static UserResponseDto ToUserResponseDto(this User user)
        {
            return user.Adapt<UserResponseDto>();
        }

        public static TaskResponseDto ToTaskResponseDto(this TaskItem task)
        {
            return task.Adapt<TaskResponseDto>();
        }

        public static IEnumerable<UserResponseDto> ToUserResponseDtos(this IEnumerable<User> users)
        {
            return users.Adapt<IEnumerable<UserResponseDto>>();
        }

        public static IEnumerable<TaskResponseDto> ToTaskResponseDtos(this IEnumerable<TaskItem> tasks)
        {
            return tasks.Adapt<IEnumerable<TaskResponseDto>>();
        }

        public static User ToUser(this UserCreateDto userCreateDto)
        {
            return userCreateDto.Adapt<User>();
        }

        public static TaskItem ToTaskItem(this TaskCreateDto taskCreateDto)
        {
            return taskCreateDto.Adapt<TaskItem>();
        }

        public static void UpdateFromDto(this User user, UserUpdateDto userUpdateDto)
        {
            userUpdateDto.Adapt(user);
        }

        public static void UpdateFromDto(this TaskItem task, TaskUpdateDto taskUpdateDto)
        {
            taskUpdateDto.Adapt(task);
        }
    }
}
