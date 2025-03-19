using BackendMultiChat.Dtos;
using BackendMultiChat.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    // Get all projects
    [HttpGet]
    public async Task<IActionResult> GetAllProjects()
    {
        try
        {
            var result = await _projectService.GetAllProjectAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // Create New Project
    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] ProjectPostDto dto)
    {
        if (dto == null)
        {
            return BadRequest("Invalid project data");
        }

        try
        {
            var result = await _projectService.CreateProjectAsync(dto);
            if (result)
                return Ok("Project created successfully");
            return BadRequest("Failed to create project");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // Update Project
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(int id, [FromBody] ProjectUpdateDto dto)
    {
        if (dto == null)
        {
            return BadRequest("Invalid project data");
        }

        try
        {
            var result = await _projectService.UpdateProjectAsync(id, dto);
            if (result)
                return Ok("Project updated successfully");
            return NotFound("Project not found");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // Delete Project
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        try
        {
            var result = await _projectService.DeleteProjectAsync(id);
            if (result)
                return Ok("Project deleted successfully");
            return NotFound("Project not found");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // View Project Overall Progress
    [HttpGet("{id}/progress/overall")]
    public async Task<IActionResult> GetProjectOverallProgress(int id)
    {
        try
        {
            var progress = await _projectService.GetProjectOverallProgressAsync(id);
            if (progress == null)
                return NotFound($"Project with ID {id} not found.");

            return Ok(progress);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }


    // View Project Member Progress
    [HttpGet("{id}/progress/members")]
    public async Task<IActionResult> GetProjectMemberProgress(int id)
    {
        try
        {
            var memberProgress = await _projectService.GetProjectMemberProgressAsync(id);
            if (memberProgress == null || !memberProgress.Any())
                return NotFound($"No member progress found for project with ID {id}.");

            return Ok(memberProgress);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

}
