using AutoMapper;
using BackendMultiChat.Data;
using BackendMultiChat.Dtos;
using BackendMultiChat.Interfaces;
using BackendMultiChat.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Security.Claims;

namespace BackendMultiChat.Services
{
    public class ProjectService : IProjectService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public ProjectService(AppDbContext context, IMapper mapper, ITokenService tokenService)
        {
            _context = context;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        public async Task<IEnumerable<ProjectGetDto>> GetAllProjectAsync()
        {
            var projects = await _context.Projects
                                         .Include(p => p.TodoLists)
                                         .ThenInclude(t => t.TodoItems)
                                         .ToListAsync();

            var projectMapping = _mapper.Map<IEnumerable<ProjectGetDto>>(projects);
            return projectMapping;
        }

        public async Task<bool> CreateProjectAsync(ProjectPostDto dto)
        {
            var project = _mapper.Map<Project>(dto);

            // Add members to project
            project.ProjectMembers = dto.MemberIds
                .Select(
                memberId => new ProjectMember
                {
                    ProjectId = project.ProjectId,
                    AccountId = memberId,
                }
                ).ToList();

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateProjectAsync(int id, ProjectUpdateDto project)
        {
            if (project == null) return false;

            var projectEntity = await _context.Projects.FindAsync(id);
            if (projectEntity == null) return false;

            _mapper.Map(project, projectEntity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteProjectAsync(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return false;

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return true;
        }

        // Get overall progress of project
        public async Task<double?> GetProjectOverallProgressAsync(int projectId)
        {
            var project = await _context.Projects
                                        .Include(p => p.TodoLists)
                                        .ThenInclude(t => t.TodoItems)
                                        .FirstOrDefaultAsync(p => p.ProjectId == projectId);

            if (project == null) return null;

            var totalTodoItems = project.TodoLists.Sum(t => t.TodoItems.Count);
            var completedTodoItems = project.TodoLists.Sum(t => t.TodoItems.Count(ti => ti.IsDone));

            var overallProgress = totalTodoItems > 0
                ? (double)completedTodoItems / totalTodoItems * 100
                : 0;

            return Math.Round(overallProgress, 2);
        }

        // Get member progress of project
        public async Task<List<MemberProgressDto>> GetProjectMemberProgressAsync(int projectId)
        {
            var project = await _context.Projects
                                        .Include(p => p.TodoLists)
                                        .ThenInclude(t => t.TodoItems)
                                        .Include(p => p.ProjectMembers)
                                        .ThenInclude(m => m.Account)
                                        .FirstOrDefaultAsync(p => p.ProjectId == projectId);

            if (project == null) return new List<MemberProgressDto>();

            var memberProgress = project.ProjectMembers
                .Select(member =>
                {
                    var memberTodoLists = project.TodoLists
                        .Where(t => t.UserId == member.AccountId);

                    var memberTotalItems = memberTodoLists.Sum(t => t.TodoItems.Count);
                    var memberCompletedItems = memberTodoLists.Sum(t => t.TodoItems.Count(ti => ti.IsDone));

                    var progress = memberTotalItems > 0
                        ? (double)memberCompletedItems / memberTotalItems * 100
                        : 0;

                    return new MemberProgressDto
                    {
                        AccountId = member.AccountId,
                        AccountName = member.Account.FullName,
                        ProgressPercentage = Math.Round(progress, 2)
                    };
                })
                .ToList();

            return memberProgress;
        }


        public async Task<List<ProjectGetSimpleDto>> GetListNameProject(string token)
        {
            var claimPrincipal = _tokenService.GetPrincipalFromToken(token);
            var userId = claimPrincipal.FindFirstValue("id");

            var projects = await _context.Projects
                .Where(p => p.ProjectMembers.Any(pm => pm.AccountId.ToString() == userId))
                .Select(p => new ProjectGetSimpleDto
                {
                    ProjectId = p.ProjectId,
                    ProjectName = p.ProjectName
                })
                .ToListAsync();
            return projects;
        }
    }
}
