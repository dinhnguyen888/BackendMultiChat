using BackendMultiChat.Dtos;
using BackendMultiChat.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private readonly IFileService _fileService;

    public FileController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpPost("send-file")]
    public async Task<IActionResult> SendFile([FromForm] FilePostDto dto)
    {
        try
        {
            var result = await _fileService.SendFile(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("view-files/{roomId}")]
    public async Task<IActionResult> ViewFiles(Guid roomId)
    {
        try
        {
            var files = await _fileService.ViewFiles(roomId);
            if (files == null || !files.Any())
            {
                return NotFound("No files found");
            }

            return Ok(files);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }

    }

    [HttpDelete("delete-file/{fileId}")]
    public async Task<IActionResult> DeleteFile(int fileId)
    {
        try
        {
            var result = await _fileService.DeleteFile(fileId);
            if (!result)
            {
                return NotFound("File not found");
            }

            return Ok(new { message = "File deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

}
