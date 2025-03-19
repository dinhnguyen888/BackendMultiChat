using BackendMultiChat.Dtos;

namespace BackendMultiChat.Interfaces
{
    public interface IProjectService
    {
        Task<bool> CreateProjectAsync(ProjectPostDto dto);
        Task<bool> DeleteProjectAsync(int id);
        Task<IEnumerable<ProjectGetDto>> GetAllProjectAsync();
        Task<List<MemberProgressDto>> GetProjectMemberProgressAsync(int projectId);
        Task<double?> GetProjectOverallProgressAsync(int projectId);
        Task<bool> UpdateProjectAsync(int id, ProjectUpdateDto project);
    }
}