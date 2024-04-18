namespace Application.Common.Interfaces;

public interface IImage
{
    long Length { get; }

    string FileName { get; }
    
    Task<string> CopyToAsync(CancellationToken cancellationToken);
}