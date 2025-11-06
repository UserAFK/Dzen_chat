using Application;
using Application.Dto;
using Microsoft.AspNetCore.SignalR;

namespace Dzen_chat.Api;

public class CommentHub : Hub
{
    private readonly CommentService _commentService;

    public CommentHub(CommentService commentService)
    {
        _commentService = commentService;
    }

    public async Task SendReply(CommentDto commentCreateDto)
    {
        await _commentService.AddCommentAsync(commentCreateDto);
        if (commentCreateDto.ParentCommentId.HasValue)
        {
            var updatedComment = await _commentService.GetCommentWithReplies(commentCreateDto.ParentCommentId.Value);
            await Clients.Group(GetGroupName(commentCreateDto.ParentCommentId.Value))
                .SendAsync("ReceiveComment", updatedComment);
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

    public async Task Test(string msg)
    {
        await Clients.Caller.SendAsync("Pong", $"Got message: {msg}");
    }

    private string GetGroupName(Guid commentId) => $"comment-{commentId}";

}
