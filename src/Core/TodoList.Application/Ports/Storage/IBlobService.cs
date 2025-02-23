using TodoList.Application.DTOs.Storage;

namespace TodoList.Application.Ports.Storage;

public interface IBlobService
{
    Task<Guid> UploadAsync(Stream stream, string contentType, CancellationToken cancellationToken = default);
    Task<FileResponseDTo> DownloadAsync(Guid fileId, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid fileId, CancellationToken cancellationToken = default);
    Task<string> GetFileUrl(Guid fileId);
}

