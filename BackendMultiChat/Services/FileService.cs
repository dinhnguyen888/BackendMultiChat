using AutoMapper;
using BackendMultiChat.Data;
using BackendMultiChat.Dtos;
using BackendMultiChat.Interfaces;
using BackendMultiChat.Models;
using Microsoft.EntityFrameworkCore;

public class FileService : IFileService
{
    private readonly AppDbContext _context;
    private readonly IMessageService _messageService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;

    public FileService(
        AppDbContext context,
        IMessageService messageService,
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper)
    {
        _context = context;
        _messageService = messageService;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
    }

    public async Task<MessageGetDto> SendFile(FilePostDto dto)
    {
        if (dto.File == null || dto.File.Length == 0)
            throw new Exception("No file selected");

        var request = _httpContextAccessor.HttpContext?.Request
                      ?? throw new InvalidOperationException("Unable to get request context");

        var fileName = Path.GetFileNameWithoutExtension(dto.File.FileName);
        var extension = Path.GetExtension(dto.File.FileName);
        var newFileName = $"{fileName}_{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", newFileName);

        // Save file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await dto.File.CopyToAsync(stream);
        }

        var fileUrl = $"{request.Scheme}://{request.Host}/uploads/{newFileName}";

        // Save file info to database
        var fileInServer = new FileStorage
        {
            FileName = newFileName,
            RoomId = dto.RoomId,
            FileUrl = fileUrl
        };
        _context.FileStorages.Add(fileInServer);

        // Create message
        var message = new MessagePostDto
        {
            SenderName = dto.SenderName,
            FileName = newFileName,
            FileUrl = fileUrl,
            RoomId = dto.RoomId,
            MessageText = $"{dto.SenderName} has sent a file: {newFileName}, click here to view: {fileUrl}"
        };

        var messageresponse = await _messageService.CreateMessageAsync(message);

        await _context.SaveChangesAsync();
        return messageresponse;
    }

    public async Task<IEnumerable<FileGetDto>> ViewFiles(Guid roomId)
    {
        var files = await _context.FileStorages
                                  .AsNoTracking()
                                  .Where(f => f.RoomId == roomId)
                                  .Select(f => new FileGetDto
                                  {
                                      FileName = f.FileName,
                                      FileUrl = f.FileUrl
                                  })
                                  .ToListAsync();

        return files;
    }

    public async Task<bool> DeleteFile(int fileId)
    {
        // Find file in database
        var file = await _context.FileStorages
                                 .FirstOrDefaultAsync(f => f.FileId == fileId);
        if (file == null) throw new Exception("File not found");

        // Delete file in server
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", file.FileName);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        // Delete file information in database
        _context.FileStorages.Remove(file);
        await _context.SaveChangesAsync();

        return true;
    }

}
