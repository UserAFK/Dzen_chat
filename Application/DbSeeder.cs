using Infrastructure;
using Infrastructure.Models;

namespace Application;

public static class DbSeeder
{
    public static void Seed(AppDbContext context)
    {
        if (context.Comments.Any())
            return;
        var comments = GetComments();
        foreach (var comment in comments)
        {
            context.Comments.Add(comment);
            // Ensure different CreatedAt timestamps
        }
        // Add seeding logic here if needed in the future
        context.SaveChanges();
    }

    private static List<Comment> GetComments()
    {
        var comments = new List<Comment>();
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var id3 = Guid.NewGuid();
        var id4 = Guid.NewGuid();
        var id5 = Guid.NewGuid();

        const string user1 = "User1";
        const string user2 = "User2";
        const string email1 = "email1@email.com";
        const string email2 = "email2@email.com";
        var comment1 = new Comment
        {
            Id = id1,
            Content = "Comment 1",
            CreatedAt = DateTime.UtcNow,
            Username = user1,
            Email = email1,
        };
        var comment2 = new Comment
        {
            Id = id2,
            ParentCommentId = id1,
            Content = "Comment 2",
            CreatedAt = DateTime.UtcNow,
            Username = user2,
            Email = email2,
        };
        var comment3 = new Comment
        {
            Id = id3,
            ParentCommentId = id2,
            Content = "Comment 3",
            CreatedAt = DateTime.UtcNow,
            Username = user1,
            Email = email1,
        };
        var comment4 = new Comment
        {
            Id = id4,
            ParentCommentId = id2,
            Content = "Comment 4",
            CreatedAt = DateTime.UtcNow,
            Username = user2,
            Email = email2,
        };
        var comment5 = new Comment
        {
            Id = id5,
            ParentCommentId = id2,
            Content = "Comment 5",
            CreatedAt = DateTime.UtcNow,
            Username = user2,
            Email = email2,
        };
        comments.Add(comment1);
        comments.Add(comment2);
        comments.Add(comment3);
        comments.Add(comment4);
        comments.Add(comment5);
        return comments;
    }
}
