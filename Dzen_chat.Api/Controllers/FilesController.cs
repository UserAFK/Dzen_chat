using Application;
using Microsoft.AspNetCore.Mvc;

namespace Dzen_chat.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilesController(FileService fileService) : ControllerBase
{

    [HttpPost("{commentId}")]
    public IActionResult AddCommentFile(Guid commentId, [FromForm] FileUploadDto file)
    {
        fileService.QueueFile(commentId, file.FormFile);
        return Ok();
    }

    [HttpGet("{commentId}")]
    public async Task<IActionResult> GetCommentFile(Guid commentId)
    {
        var (filetype, filedata, filename) = await fileService.GetFileAsync(commentId);
        if (filedata.Length == 0)
            return NotFound();
        return File(filedata, filetype, filename);
    }

    public class FileUploadDto
    {
        public IFormFile FormFile { get; set; } = default!;
    }
}
