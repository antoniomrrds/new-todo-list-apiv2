using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using TodoList.Application.DTOs.Storage;
using TodoList.Application.Ports.Storage;

namespace TodoList.Infrastructure.Storage;
internal sealed class BlobService(BlobServiceClient blobServiceClient, IConfiguration configuration)
    : IBlobService
{
    private readonly string _containerName = configuration["StorageSettings:ContainerName"] ?? throw new InvalidOperationException("Container name is missing");


    public async Task<Guid> UploadAsync(Stream stream, string contentType, CancellationToken cancellationToken = default)
    {
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
        var fileId = Guid.NewGuid();
        BlobClient blobClient = containerClient.GetBlobClient(fileId.ToString());
        await blobClient.UploadAsync(
            stream,
            new BlobHttpHeaders { ContentType = contentType },
            cancellationToken: cancellationToken
            );
        return fileId;
    }

    public async Task<FileResponseDTo> DownloadAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
        BlobClient blobClient = containerClient.GetBlobClient(fileId.ToString());
        Response<BlobDownloadResult>? response = await blobClient.DownloadContentAsync(cancellationToken: cancellationToken);
        return new FileResponseDTo(response.Value.Content.ToStream(), response.Value.Details.ContentType);
    }

    public async Task DeleteAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
        BlobClient blobClient = containerClient.GetBlobClient(fileId.ToString());
        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }

    public Task<string> GetFileUrl(Guid fileId)
    {
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
        BlobClient blobClient = containerClient.GetBlobClient(fileId.ToString());
        return Task.FromResult(blobClient.Uri.ToString());
    }
}
