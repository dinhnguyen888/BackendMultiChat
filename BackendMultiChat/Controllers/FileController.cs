using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BackendMultiChat.Data;
using BackendMultiChat.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using BackendMultiChat.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class FileController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IHubContext<MessageHub> _messageHubContext;

    public FileController(AppDbContext context, IHubContext<MessageHub> messageHubContext)
    {
        _context = context;
        _messageHubContext = messageHubContext;
    }

    [HttpPost("SendFile")]
    public async Task<IActionResult> SendFile([FromForm] IFormFile file, [FromForm] string conversationId, [FromForm] string fromNumber, [FromForm] string senderName)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file selected");

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", file.FileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{file.FileName}";

        var fileInServer = new FileSaveInServer
        {
            FileName = file.FileName,
            ConversationID = Convert.ToInt32(conversationId),
            FileUrl = fileUrl
        };

        _context.FileSaveInServers.Add(fileInServer);
        await _context.SaveChangesAsync();

        var message = new Message
        {
            FromNumber = fromNumber,
            SentDateTime = DateTime.Now,
            FileName = file.FileName,
            FileUrl = fileUrl,
            ConversationId = Convert.ToInt32(conversationId),
            MessageText = $"{senderName} đã gửi file: {file.FileName}"
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        await _messageHubContext.Clients.Group(conversationId.ToString())
            .SendAsync("ReceiveMessage", fromNumber, message.MessageText, DateTime.Now);

        return Ok(new { message = "Gửi file thành công! đường link:", fileUrl });
    }


    [HttpGet("view-file/{conversationId}")]
    public async Task<ActionResult<FileSaveInServer>> GetFileInServer(int conversationId)
    {

  
        var fileList = await _context.FileSaveInServers
                                     .Where(f => f.ConversationID == conversationId)
                                     .ToListAsync();

        if (fileList == null || !fileList.Any())
        {
            return NotFound();
        }

        return Ok(fileList);

    }
}
