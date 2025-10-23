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
        return await context.Comments.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task AddCommentAsync(CommentDto comment, CancellationToken cancellationToken = default)
    {
        await context.Comments.AddAsync(mapper.Map<Comment>(comment), cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}
