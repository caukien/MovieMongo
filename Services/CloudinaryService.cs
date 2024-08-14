using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MovieMongo.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class CloudinaryService
{
    private readonly Cloudinary _cloudinary;
    private readonly string _folder;
    private readonly long _maxFileSize = 3 * 1024 * 1024;
    private readonly string[] _allowedFormats = { ".jpg", ".png", ".webp" };

    public CloudinaryService(IOptions<CloudinarySettings> cloudinarySettings, IConfiguration configuration)
    {
        var account = new Account(
            cloudinarySettings.Value.CLOUDINARY_NAME,
            cloudinarySettings.Value.CLOUDINARY_KEY,
            cloudinarySettings.Value.CLOUDINARY_SECRET);

        _cloudinary = new Cloudinary(account);
        _folder = configuration["Cloudinary:FOLDER"];
    }

    public async Task<ImageUploadResult> UploadImageAsync(IFormFile file)
    {
        var uploadResult = new ImageUploadResult();
        if (file.Length > 0)
        {
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!_allowedFormats.Contains(extension))
            {
                throw new InvalidOperationException("Unsupported file format. Only jpg, png, and webp are allowed.");
            }

            if (file.Length > _maxFileSize)
            {
                throw new InvalidOperationException("File size exceeds the 3MB limit.");
            }
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                Folder = _folder
            };
            uploadResult = await _cloudinary.UploadAsync(uploadParams);
        }
        return uploadResult;
    }

    public async Task<DeletionResult> DeleteImageAsync(string publicId)
    {
        var deletionParams = new DeletionParams(publicId);
        return await _cloudinary.DestroyAsync(deletionParams);
    }
}