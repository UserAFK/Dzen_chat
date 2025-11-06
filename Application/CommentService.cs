using Application.Dto;
using AutoMapper;
using Ganss.Xss;
using Infrastructure;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Application;

public class CommentService
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IBackgroundTaskQueue _queue;

    public CommentService(IAppDbContext context, IMapper mapper, IBackgroundTaskQueue queue)
    {
        _context = context;
        _mapper = mapper;
        _queue = queue;
    }

    public async Task<List<Comment>> GetCommentsAsync(int? page, string? orderBy, string? order)
    {
        int pageSize = 25;
        var query = _context.Comments
            .Where(c => c.ParentCommentId == null)
            .Skip(page.HasValue && page.Value > 0 ? (page.Value - 1) * pageSize : 0)
            .Take(pageSize);
        query = (orderBy?.ToLower()) switch
        {
            "username" => order?.ToLower() == "desc"
                                ? query.OrderByDescending(c => c.Username)
                                : query.OrderBy(c => c.Username),
            "email" => order?.ToLower() == "desc"
                                ? query.OrderByDescending(c => c.Email)
                                : query.OrderBy(c => c.Username),
            "createdat" => order?.ToLower() == "desc"
                                ? query.OrderByDescending(c => c.CreatedAt)
                                : query.OrderBy(c => c.CreatedAt),
            _ => query.OrderByDescending(c => c.CreatedAt),
        };
        query = query
            .AsNoTracking()
            .Select(c=>new Comment()
            {
                Id = c.Id,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                Username = c.Username,
                Email = c.Email,
                ParentCommentId = c.ParentCommentId,
                FileData = c.FileData,
                FileType = c.FileType,
            });
        return await query.ToListAsync();
    }

    public async Task<Comment?> GetCommentWithReplies(Guid selectedCommentId)
    {
        var commentProjection = await _context.Comments
            .AsNoTracking()
            .Where(c => c.Id == selectedCommentId)
            .Select(c => new Comment
            {
                Id = c.Id,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                Username = c.Username,
                Email = c.Email,
                ParentCommentId = c.ParentCommentId,
                FileData = c.FileData,
                FileType = c.FileType,

                ParentComment = c.ParentComment == null ? null : new Comment
                {
                    Id = c.ParentComment.Id,
                    Content = c.ParentComment.Content,
                    CreatedAt = c.ParentComment.CreatedAt,
                    Username = c.ParentComment.Username,
                    Email = c.ParentComment.Email,
                },

                Replies = c.Replies.Select(r => new Comment
                {
                    Id = r.Id,
                    Content = r.Content,
                    CreatedAt = r.CreatedAt,
                    Username = r.Username,
                    Email = r.Email,
                    ParentCommentId = r.ParentCommentId,

                    ParentComment = null,
                    Replies = new List<Comment>()
                }).ToList()
            })
            .FirstOrDefaultAsync();

        return commentProjection;
    }

    public async Task AddCommentAsync(CommentDto comment)
    {
        if (string.IsNullOrWhiteSpace(comment.Email) || string.IsNullOrWhiteSpace(comment.Username))
            throw new ArgumentNullException("Username and Email are required to add a comment.");
        if (string.IsNullOrWhiteSpace(comment.Content))
            throw new ArgumentNullException("Content is required to add a comment.");

        comment.Content = CleanContent(comment.Content);
        var newComment = _mapper.Map<Comment>(comment);
        newComment.FileType = comment.File?.ContentType;

        await _context.Comments.AddAsync(newComment);
        await _context.SaveChangesAsync();

        if (comment.File != null)
        {
            using var ms = new MemoryStream();
            await comment.File.CopyToAsync(ms);

            var item = new FileProcessingItem(
                newComment.Id,
                comment.File.ContentType,
                ms.ToArray()
            );

            _queue.QueueFile(item);
        }
    }

    private string CleanContent(string html)
    {
        var sanitizer = new HtmlSanitizer();
        sanitizer.AllowedTags.Clear();
        sanitizer.AllowedTags.Add("a");
        sanitizer.AllowedTags.Add("i");
        sanitizer.AllowedTags.Add("strong");
        sanitizer.AllowedTags.Add("code");

        sanitizer.AllowedAttributes.Add("href");
        sanitizer.AllowedAttributes.Add("title");

        return sanitizer.Sanitize(html);
    }
}
