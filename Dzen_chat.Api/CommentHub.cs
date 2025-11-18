using Application;
using Application.Dto;
using Dzen_chat.Api.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.SignalR;

namespace Dzen_chat.Api;

[EnableCors("LocalPolicy")]
public class CommentHub : Hub
{
    private readonly CommentService _commentService;
    private readonly CaptchaService _captchaService;

    public CommentHub(CommentService commentService, CaptchaService captchaService)
    {
        _commentService = commentService;
        _captchaService = captchaService;
    }

    public async Task SendReply(CommentNewDto commentNewDto)
    {
        if (!await _captchaService.VerifyRecaptchaAsync(commentNewDto.Recaptcha))
        {
            throw new UnauthorizedAccessException("Recaptcha verification failed.");
        }
        try
        {
            await _commentService.AddCommentAsync(commentNewDto);

            if (commentNewDto.ParentCommentId.HasValue)
            {
                var updatedComment = await _commentService.GetCommentWithReplies(commentNewDto.ParentCommentId.Value);
                await Clients.Group(GetGroupName(commentNewDto.ParentCommentId.Value))
                    .SendAsync("ReceiveComment", updatedComment);
            }
        }
        catch (Exception e)
        {
            await Clients.Caller.SendAsync("Error", e.Message);
        }
    }

    public async Task JoinCommentGroup(string commentId)
    {
        var id = Guid.Parse(commentId);
        var comment = await _commentService.GetCommentWithReplies(id);
        if (comment == null)
        {
            await Clients.Caller.SendAsync("Error", $"Comment with ID {commentId} not found.");
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, GetGroupName(id));
        await Clients.Caller.SendAsync("JoinedCommentGroup", comment);
    }

    public async Task LeaveCommentGroup(Guid commentId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, commentId.ToString());
    }

    private string GetGroupName(Guid commentId) => $"comment-{commentId}";
}
