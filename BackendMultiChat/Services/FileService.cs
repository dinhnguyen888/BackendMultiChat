using AutoMapper;
using BackendMultiChat.Data;
using BackendMultiChat.Dtos;
using BackendMultiChat.Interfaces;
using BackendMultiChat.Models;
using BackendMultiChat.Services;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Security.Claims;

public class FileService : IFileService
{
    private readonly AppDbContext _context;
    private readonly IMessageService _messageService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;
    private readonly IDirectMessageService _dmMessage;
    private readonly ITokenService _tokenService;
    private readonly IFileHandler _fileHandler;

    public FileService(
        AppDbContext context,
        IMessageService messageService,
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper,
        IDirectMessageService dmMessage,
        ITokenService tokenService,
        IFileHandler fileHandler)
    {
        _context = context;
        _messageService = messageService;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
        _dmMessage = dmMessage;
        _tokenService = tokenService;
        _fileHandler = fileHandler;
    }

    public async Task<RMGetDto> SendFileToRM(FilePostToRoomDto dto)
    {
        var (newFileName, fileUrl) = await SaveFileInServer(dto.File);

        // Save file info to database
        var fileInServer = new FileStorage
        {
            FileName = newFileName,
            RoomId = dto.RoomId,
            FileUrl = fileUrl
        };
        _context.FileStorages.Add(fileInServer);

        // Create message
        var message = new RMPostDto
        {
            SenderName = dto.SenderName,
            FileName = newFileName,
            FileUrl = fileUrl,
            RoomId = dto.RoomId,
            MessageText = $"{dto.SenderName} has sent a file: {newFileName}, click here to view: {fileUrl}"
        };

        var messageResponse = await _messageService.CreateMessageAsync(message);

        await _context.SaveChangesAsync();
        return messageResponse;
    }

    public async Task<IEnumerable<FileGetDto>> ViewFilesInRM(Guid roomId)
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

    public async Task<bool> DeleteFileInRM(int fileId)
    {
        // Find file in database
        var file = await _context.FileStorages
                                 .FirstOrDefaultAsync(f => f.FileId == fileId);
        if (file == null) throw new Exception("File not found");

        // Delete file in server
        _fileHandler.DeleteFileInServer(file.FileName);

        // Delete file information in database
        _context.FileStorages.Remove(file);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<DMGetDto> SendFileToDM(IFormFile file, Guid senderId, Guid receiverId)
    {
        var (newFileName, fileUrl) = await SaveFileInServer(file);

        // Create message
        var message = new DMPostDto
        {
            SenderId = senderId,
            FileName = newFileName,
            FileUrl = fileUrl,
            ReceiverId = receiverId,
            MessageText = $"new file: {newFileName}, click here to view: {fileUrl}"
        };
        var dmMessage = await _dmMessage.CreateDirectMessageAsync(message);

        await _context.SaveChangesAsync();
        return dmMessage;
    }

    public async Task<IEnumerable<FileGetDto>> ViewFileInDM(string token, Guid receiver)
    {
        // Get senderId from token
        var claimPrincipal = _tokenService.GetPrincipalFromToken(token);
        var sender = Guid.Parse(claimPrincipal.FindFirstValue("id"));
      
        var files = await _context.DirectMessages
                                  .AsNoTracking()
                                  .Where(f => f.SenderId == sender && f.ReceiverId == receiver)
                                  .Select(f => new FileGetDto
                                  {
                                      FileName = f.FileName,
                                      FileUrl = f.FileUrl
                                  })
                                  .ToListAsync();

        return files ?? Enumerable.Empty<FileGetDto>();
    }





    private async Task<(string, string)> SaveFileInServer(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new Exception("No file selected");

        var request = _httpContextAccessor.HttpContext?.Request
                      ?? throw new InvalidOperationException("Unable to get request context");

        
        var fileName = Path.GetFileNameWithoutExtension(file.FileName)
                          .Replace(" ", "");
                        

        var extension = Path.GetExtension(file.FileName);
        var newFileName = $"{fileName}_{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", newFileName);

        // Save file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var fileUrl = $"{request.Scheme}://{request.Host}/uploads/{newFileName}";
        return (newFileName, fileUrl);
    }

}
