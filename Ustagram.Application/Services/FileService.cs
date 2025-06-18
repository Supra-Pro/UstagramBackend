using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Ustagram.Application.Abstractions;

namespace Ustagram.Application.Services;

public class FileService : IFileService
{
    private readonly IHostEnvironment _environment;

    public FileService(IHostEnvironment environment)
    {
        _environment = environment;
    }
    
    public async Task<string> SaveFileAsync(IFormFile file, string subDirectory)
    {
        try
        {
            if (file == null || file.Length == 0)
                return null;

            // Create directory if it doesn't exist
            var uploadsPath = Path.Combine(_environment.ContentRootPath, subDirectory);
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            // Generate unique filename
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Path.Combine(subDirectory, fileName).Replace("\\", "/");
        }
        catch (Exception ex)
        {
            // Log error here
            throw new Exception("File upload failed", ex);
        }
    }

    public FileStream GetFile(string filePath)
    {
        var fullPath = Path.Combine(_environment.ContentRootPath, filePath);
        if (!System.IO.File.Exists(fullPath))
            return null;

        return new FileStream(fullPath, FileMode.Open, FileAccess.Read);
    }

    public bool DeleteFile(string filePath)
    {
        var fullPath = Path.Combine(_environment.ContentRootPath, filePath);
        if (!System.IO.File.Exists(fullPath))
            return false;

        System.IO.File.Delete(fullPath);
        return true;
    }
    
}