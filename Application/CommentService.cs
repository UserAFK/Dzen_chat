using Application.Dto;
using AutoMapper;
using Infrastructure;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Application;

public class CommentService(IAppDbContext context, IMapper mapper)
{
    public async Task<List<Comment>> GetCommentsAsync(CancellationToken cancellationToken = default)
    {
        return await context.Comments.AsNoTracking().Take(25).ToListAsync(cancellationToken);
    }

    public async Task<Comment?> GetCommentWithLimitedReplies(Guid targetCommentId, CancellationToken cancellationToken = default)
    {
        var commentProjection = await context.Comments
            .AsNoTracking()
            .Where(c => c.Id == targetCommentId)
            .Select(c => new Comment
            {
                Id = c.Id,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                Username = c.Username,
                ParentCommentId = c.ParentCommentId,

                ParentComment = c.ParentComment == null ? null : new Comment
                {
                    Id = c.ParentComment.Id,
                    Content = c.ParentComment.Content,
                    CreatedAt = c.ParentComment.CreatedAt,
                    Username = c.ParentComment.Username,
                },

                Replies = c.Replies.Select(r => new Comment
                {
                    Id = r.Id,
                    Content = r.Content,
                    CreatedAt = r.CreatedAt,
                    Username = r.Username,
                    ParentCommentId = r.ParentCommentId,

                    ParentComment = null,
                    Replies = new List<Comment>()
                }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        return commentProjection;
    }

    public async Task AddCommentAsync(CommentDto comment, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(comment.Email) || string.IsNullOrWhiteSpace(comment.Username))
            throw new ArgumentNullException("Username and Email are required to add a comment.");

        var newComment = mapper.Map<Comment>(comment);
        await context.Comments.AddAsync(newComment, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}
