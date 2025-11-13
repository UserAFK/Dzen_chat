using Application.Dto;
using Ganss.Xss;
using Infrastructure;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Application;

public class CommentService
{
    private readonly IAppDbContext _context;
    private readonly FileService _fileService;

    public CommentService(IAppDbContext context, FileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    public async Task<List<CommentTableDto>> GetCommentsAsync(int? page, string? orderBy, string? order)
    {
        int pageSize = 25;
        var query = _context.Comments
            .Where(c => c.ParentCommentId == null)
            .Skip(page.HasValue && page.Value > 0 ? (page.Value - 1) * pageSize : 0)
            .Include(c => c.User)
            .Select(c => new CommentTableDto()
            {
                Id = c.Id,
                CreatedAt = c.CreatedAt,
                Username = c.User.Username,
                Email = c.User.Email,
                FileType = c.FileType,
            })
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
            .AsNoTracking();
        return await query.ToListAsync();
    }

    public async Task<CommentDto?> GetCommentWithReplies(Guid selectedCommentId)
    {
        var commentProjection = await _context.Comments
            .AsNoTracking()
            .Where(c => c.Id == selectedCommentId)
            .Include(c => c.User)
            .Include(c => c.Replies)
            .Select(c => new CommentDto
            {
                Id = c.Id,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                Username = c.User.Username,
                Email = c.User.Email,
                ParentCommentId = c.ParentCommentId,
                FileType = c.FileType,

                ParentComment = c.ParentComment == null ? null : new CommentReplyDto
                {
                    Id = c.ParentComment.Id,
                    Content = c.ParentComment.Content,
                    CreatedAt = c.ParentComment.CreatedAt,
                    Username = c.ParentComment.User.Username,
                    Email = c.ParentComment.User.Email,
                },

                Replies = c.Replies.Select(r => new CommentReplyDto
                {
                    Id = r.Id,
                    Content = r.Content,
                    CreatedAt = r.CreatedAt,
                    Username = r.User.Username,
                    Email = r.User.Email,
                    ParentCommentId = r.ParentCommentId
                }).ToList()
            })
            .FirstOrDefaultAsync();

        return commentProjection;
    }

    public async Task AddCommentAsync(CommentNewDto comment)
    {
        if (string.IsNullOrWhiteSpace(comment.Email) || string.IsNullOrWhiteSpace(comment.Username))
            throw new ArgumentNullException("Username and Email are required to add a comment.");
        if (string.IsNullOrWhiteSpace(comment.Content))
            throw new ArgumentNullException("Content is required to add a comment.");

        comment.Content = CleanContent(comment.Content);
        var newComment = new Comment()
        {
            Id = comment.Id.Value,
            Content = comment.Content,
            ParentCommentId = comment.ParentCommentId
        };
        newComment.CreatedAt = DateTime.UtcNow;
        var user = new User();
        user = await _context.Users.FirstOrDefaultAsync(u => u.Username == comment.Username && u.Email == comment.Email);
        if (user == null)
        {
            user = new User
            {
                Id = Guid.NewGuid(),
                Username = comment.Username!,
                Email = comment.Email!,
                HomePage = comment.HomePage
            };

            await _context.Users.AddAsync(user);
        }
        
        newComment.FileType = comment.File?.ContentType;
        newComment.UserId = user.Id;

        await _context.Comments.AddAsync(newComment);
        await _context.SaveChangesAsync();

        if (comment.File != null)
        {
            _fileService.QueueFile(newComment.Id, comment.File);
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
