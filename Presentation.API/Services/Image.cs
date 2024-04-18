using Application.Common.Interfaces;

namespace Presentation.API.Services;

public class Image(IFormFile formFile) : IImage
{
    public long Length { get; } = formFile.Length;

    public string FileName { get; } = formFile.FileName;

    public async Task<string> CopyToAsync(CancellationToken cancellationToken)
    {
        var filePath = Path.Combine("Images", FileName);
            
        if (!Directory.Exists("Images"))
        {
            Directory.CreateDirectory("Images");
        }

        await using var fileStream = new FileStream(filePath, FileMode.Create);
        await formFile.CopyToAsync(fileStream, cancellationToken);
        
        return filePath;
    }

}