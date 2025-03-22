using BackendMultiChat.Dtos;
using BackendMultiChat.Interfaces;
using Microsoft.AspNetCore.Authorization;
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

    [HttpPost("send-file-to-room")]
    public async Task<IActionResult> SendFile([FromForm] FilePostToRoomDto dto)
    {
        try
        {
            var result = await _fileService.SendFileToRM(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("view-files-in-room/{roomId}")]
    public async Task<IActionResult> ViewFiles(Guid roomId)
    {
        try
        {
            var files = await _fileService.ViewFilesInRM(roomId);
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
    //[Authorize(Policy = "AdminOnly")]
    [HttpDelete("delete-file-in-room/{fileId}")]
    public async Task<IActionResult> DeleteFile(int fileId)
    {
        try
        {
            var result = await _fileService.DeleteFileInRM(fileId);
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

    [HttpPost("send-file-to-dm")]
    public async Task<IActionResult> SendFileToDM([FromForm] FilePostToDMDto dto)
    {
        try
        {
            var result = await _fileService.SendFileToDM(dto.File, dto.Sender, dto.Receiver);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("view-files-in-dm/{roomId}")]
    public async Task<IActionResult> ViewFilesInDM([FromQuery] Guid receiver)
    {
        try
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token))
                return Unauthorized("Token is missing");
            

            var files = await _fileService.ViewFileInDM(token,receiver);
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

  


}
