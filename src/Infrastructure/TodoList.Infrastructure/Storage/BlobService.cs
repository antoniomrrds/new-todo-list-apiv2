using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using System.Net;
using TodoList.Application.DTOs.Storage;
using TodoList.Application.Ports.Storage;
using TodoList.Infrastructure.Helpers;

namespace TodoList.Infrastructure.Storage;

internal sealed class BlobService(BlobServiceClient blobServiceClient, IConfiguration configuration)
    : IBlobService
{
    private readonly string _containerName = configuration["StorageSettings:ContainerName"]
        ?? throw new InvalidOperationException("Container name is missing");

    // Método que retorna o BlobContainerClient e verifica a existência do container
    private async Task<BlobContainerClient> GetContainerClientAsync(CancellationToken cancellationToken)
    {
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_containerName);


        bool exists = await containerClient.ExistsAsync(cancellationToken);

        // Log para verificar o resultado da verificação
        if (exists) return containerClient;
        throw new CustomHttpException("O container especificado não existe.", HttpStatusCode.NotFound);

    }


    public async Task<Guid> UploadAsync(Stream stream, string contentType, CancellationToken cancellationToken = default)
    {
        try
        {
            BlobContainerClient containerClient = await GetContainerClientAsync(cancellationToken);

            var fileId = Guid.NewGuid();
            BlobClient blobClient = containerClient.GetBlobClient(fileId.ToString());

            await blobClient.UploadAsync(
                stream,
                new BlobHttpHeaders { ContentType = contentType },
                cancellationToken: cancellationToken
            );

            return fileId;
        }
        catch (RequestFailedException ex)
        {
            throw new CustomHttpException("Falha no upload da imagem. Verifique a conexão com o Blob Storage.", HttpStatusCode.ServiceUnavailable);
        }
        catch (CustomHttpException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new CustomHttpException("Erro inesperado ao enviar a imagem.", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<FileResponseDTo> DownloadAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        try
        {
            BlobContainerClient containerClient = await GetContainerClientAsync(cancellationToken);
            BlobClient blobClient = containerClient.GetBlobClient(fileId.ToString());

            if (!await blobClient.ExistsAsync(cancellationToken))
            {
                throw new CustomHttpException($"Arquivo com ID {fileId} não encontrado.", HttpStatusCode.NotFound);
            }

            Response<BlobDownloadResult> response = await blobClient.DownloadContentAsync(cancellationToken: cancellationToken);
            return new FileResponseDTo(response.Value.Content.ToStream(), response.Value.Details.ContentType);
        }
        catch (CustomHttpException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new CustomHttpException("Erro ao baixar a imagem.", HttpStatusCode.InternalServerError);
        }
    }

    public async Task DeleteAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        try
        {
            BlobContainerClient containerClient = await GetContainerClientAsync(cancellationToken);
            BlobClient blobClient = containerClient.GetBlobClient(fileId.ToString());

            // Log de verificação de existência
            bool exists = await blobClient.ExistsAsync(cancellationToken);
            if (!exists)
            {
                throw new CustomHttpException($"Arquivo com ID {fileId} não encontrado para exclusão.", HttpStatusCode.NotFound);
            }

            // Tentativa de exclusão
            bool deleted = await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);

            if (!deleted)
            {
                throw new CustomHttpException($"Erro ao excluir o arquivo com ID {fileId}.", HttpStatusCode.InternalServerError);
            }

            // Log de sucesso
            Console.WriteLine($"Arquivo com ID {fileId} excluído com sucesso.");
        }
        catch (CustomHttpException ex)
        {
            // Log detalhado da exceção
            Console.WriteLine($"Erro específico ao tentar excluir o arquivo: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            // Log de erro genérico
            Console.WriteLine($"Erro inesperado ao tentar excluir o arquivo: {ex.Message}");
            throw new CustomHttpException("Erro ao deletar o arquivo.", HttpStatusCode.InternalServerError);
        }

    }

    public async Task<string> GetFileUrl(Guid fileId, CancellationToken cancellationToken = default)
    {
        try
        {
            BlobContainerClient containerClient = await GetContainerClientAsync(cancellationToken);
            BlobClient blobClient = containerClient.GetBlobClient(fileId.ToString());

            return blobClient.Uri.ToString();
        }
        catch (CustomHttpException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new CustomHttpException("Erro ao obter a URL da imagem.", HttpStatusCode.InternalServerError);
        }
    }
}
