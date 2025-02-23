using Microsoft.AspNetCore.Mvc;
using TodoList.Application.DTOs.Storage;
using TodoList.Application.Ports.Storage;

namespace TodoList.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageUploadController : ControllerBase
    {
        private readonly  IBlobService _blobService;

        public ImageUploadController(IBlobService blobService)
        {
            _blobService = blobService;
        }

        // Endpoint para upload da imagem
        [HttpPost("files")]
        public async Task<IActionResult> UploadImage(IFormFile file )
        {

            await using Stream stream = file.OpenReadStream();

            Guid fileId = await _blobService.UploadAsync(stream, file.ContentType);
            return Ok(fileId);
        }

        // Endpoint para download da imagem
        [HttpGet("files/{fileId}")]
        public async Task<IActionResult> DownloadImage([FromRoute] Guid fileId)
        {
            FileResponseDTo fileResponse = await _blobService.DownloadAsync(fileId);
            return File(fileResponse.Stream, fileResponse.ContentType);
        }

        // Endpoint para deletar a imagem
        [HttpDelete("files/{fileId}")]
        public async Task<IActionResult> DeleteImage([FromRoute] Guid fileId)
        {
            await _blobService.DeleteAsync(fileId);
            return NoContent();
        }

        // Endpoint para obter a URL da imagem
        [HttpGet("files/{fileId}/url")]
        public async Task<IActionResult> GetFileUrl([FromRoute] Guid fileId)
        {
            Console.WriteLine(Environment.GetEnvironmentVariable("BLOB_STORAGE_CONTAINER_NAME"));
            string fileUrl = await _blobService.GetFileUrl(fileId);
            return Ok(fileUrl);
        }
    }
}
