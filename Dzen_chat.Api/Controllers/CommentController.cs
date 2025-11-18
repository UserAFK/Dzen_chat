using Application;
using Application.Dto;
using Dzen_chat.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Dzen_chat.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentController(CommentService commentService, CaptchaService captchaService) : ControllerBase
{

    [HttpGet()]
    public async Task<IActionResult> GetComments([FromQuery] int? page, string? orderBy, string? order)
    {
        var comments = await commentService.GetCommentsAsync(page, orderBy, order);
        return Ok(comments);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCommentById(Guid id)
    {
        var comment = await commentService.GetCommentWithReplies(id);
        return Ok(comment);
    }

    [HttpPost]
    [RequestSizeLimit(2_000_000)]
    public async Task<IActionResult> AddComment([FromForm] CommentNewDto comment)
    {
        if (!await captchaService.VerifyRecaptchaAsync(comment.Recaptcha))
        {
            throw new UnauthorizedAccessException("Recaptcha verification failed.");
        }
        try
        {
            await commentService.AddCommentAsync(comment);
            return Ok();
        }
        catch(Exception e)
        {
            return Problem(detail: e.Message,statusCode: StatusCodes.Status400BadRequest);
        }
    }
}
