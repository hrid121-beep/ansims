using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace IMS.Application.Services
{
    public class FileService : IFileService
    {
        private readonly string _webRootPath;
        private readonly string _contentRootPath;

        public FileService(string webRootPath, string contentRootPath)
        {
            _webRootPath = webRootPath;
            _contentRootPath = contentRootPath;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid file");

            var uploadsFolder = Path.Combine(_webRootPath, "uploads", folder);
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return Path.Combine("uploads", folder, uniqueFileName).Replace("\\", "/");
        }

        public async Task<bool> DeleteFileAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            var fullPath = Path.Combine(_webRootPath, filePath);
            if (File.Exists(fullPath))
            {
                await Task.Run(() => File.Delete(fullPath));
                return true;
            }

            return false;
        }

        public async Task<byte[]> GetFileAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;

            var fullPath = Path.Combine(_webRootPath, filePath);
            if (File.Exists(fullPath))
            {
                return await File.ReadAllBytesAsync(fullPath);
            }

            return null;
        }

        public Task<string> GetFileUrlAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return Task.FromResult<string>(null);

            // Return the relative URL path
            return Task.FromResult($"/{filePath.Replace("\\", "/")}");
        }

        public bool IsValidImageFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                return false;

            // Check MIME type
            var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/bmp", "image/webp" };
            return allowedMimeTypes.Contains(file.ContentType.ToLowerInvariant());
        }

        public bool IsValidDocumentFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt", ".csv" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                return false;

            // Check MIME type
            var allowedMimeTypes = new[]
            {
                "application/pdf",
                "application/msword",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "application/vnd.ms-excel",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "text/plain",
                "text/csv"
            };

            return allowedMimeTypes.Contains(file.ContentType.ToLowerInvariant());
        }

        public async Task<string> SaveBase64ImageAsync(string base64String, string folder, string fileName)
        {
            if (string.IsNullOrEmpty(base64String))
                throw new ArgumentException("Invalid base64 string");

            // Remove data:image/jpeg;base64, prefix if present
            var base64Data = base64String;
            if (base64String.Contains(","))
            {
                base64Data = base64String.Split(',')[1];
            }

            var bytes = Convert.FromBase64String(base64Data);

            var uploadsFolder = Path.Combine(_webRootPath, "uploads", folder);
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{fileName}_{Guid.NewGuid()}.png";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            await File.WriteAllBytesAsync(filePath, bytes);

            return Path.Combine("uploads", folder, uniqueFileName).Replace("\\", "/");
        }


    }
}