using BackendMultiChat.Data;
using BackendMultiChat.Hubs;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
var builder = WebApplication.CreateBuilder(args);

// Cấu hình giới hạn kích thước cho form và tệp tải lên
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 50 * 1024 * 1024; // Giới hạn 50 MB
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BackendMultiChat", Version = "v1" });

    // Cấu hình hỗ trợ upload file
    c.OperationFilter<SwaggerFileOperationFilter>();
});

builder.Services.AddSignalR();

// Cấu hình DbContext với MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 27)))); // Thay đổi phiên bản MySQL nếu cần

var app = builder.Build();

// Kiểm tra kết nối tới MySQL
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        if (dbContext.Database.CanConnect())
        {
            logger.LogInformation("MYSQL:OK.");
        }
        else
        {
            logger.LogError("MYSQL:FAIL");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "ERROR:");
    }
}

// Configure the HTTP request pipeline.

// Cấu hình middleware cho môi trường phát triển
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Phục vụ các tệp tĩnh (wwwroot)


// Định tuyến
app.UseRouting();

// Cấu hình HTTPS redirection
app.UseHttpsRedirection();

// Cấu hình Authorization và Authentication (nếu có)
app.UseAuthorization();

// Đăng ký các route trực tiếp
app.MapControllers();
app.UseStaticFiles();
app.MapHub<MessageHub>("/messagehub");
app.MapHub<StatusHub>("/statushub");
app.MapHub<NotificationHub>("/notificationhub");

app.Run();
