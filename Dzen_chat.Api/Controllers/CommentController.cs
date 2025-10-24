using Application;
using Application.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Dzen_chat.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CommentController(CommentService commentService) : ControllerBase
{

    [HttpGet()]
    public async Task<IActionResult> GetComments()
    {
        var comments = await commentService.GetCommentsAsync();
        return Ok(new { comments });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCommentById(Guid id)
    {
        var comment = await commentService.GetCommentWithLimitedReplies(id);
        return Ok(new { comment });
    }

    [HttpPost]
    public async Task<IActionResult> AddComment([FromBody] CommentDto comment)
    {
        await commentService.AddCommentAsync(comment);
        return Ok("Comment added successfully.");
    }
}
