using Application;
using Application.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Dzen_chat.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentController(CommentService commentService, FileService fileService) : ControllerBase
{

    [HttpGet()]
    public async Task<IActionResult> GetComments([FromQuery] int? page, string? orderBy, string? order)
    {
        var comments = await commentService.GetCommentsAsync(page, orderBy, order);
        return Ok( comments );
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCommentById(Guid id)
    {
        var comment = await commentService.GetCommentWithReplies(id);
        return Ok( comment );
    }

    [HttpPost]
    [RequestSizeLimit(2_000_000)]
    public async Task<IActionResult> AddComment([FromForm] CommentDto comment)
    {
        await commentService.AddCommentAsync(comment);
        return Ok("Comment added successfully.");
    }

    [HttpGet("{id}/file")]
    public async Task<IActionResult> GetCommentFile(Guid id)
    {
        var (filetype, filedata, filename) = await fileService.GetFileAsync(id);
        if (filedata.Length == 0)
            return NotFound("File not found.");
        return File(filedata, filetype, filename);
    }
}
