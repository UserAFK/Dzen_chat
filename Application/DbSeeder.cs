using Infrastructure;
using Infrastructure.Models;

namespace Application;

public static class DbSeeder
{
    public static void Seed(AppDbContext context)
    {

        if (context.Users.Any() || context.Comments.Any())
            return;
        var user1Id = Guid.NewGuid();
        var user2Id = Guid.NewGuid();
        var user3Id = Guid.NewGuid();
        var users = new List<User>
            {
                new User
                {
                    Id = user1Id,
                    Username = "User1",
                    Email = "user1@example.com"
                },
                new User
                {
                    Id = user2Id,
                    Username = "User2",
                    Email = "user2@example.com"
                },
                new User
                {
                    Id = user3Id,
                    Username = "User3",
                    Email = "user3@example.com"
                }
            };
        context.Users.AddRange(users);
        context.SaveChanges();

        var comments = new List<Comment>
            {
                new Comment
                {
                    Id = Guid.NewGuid(),
                    Content = "User 1 comment",
                    UserId = user1Id,
                    CreatedAt = DateTime.UtcNow.AddMinutes(-30)
                },
                new Comment
                {
                    Id = Guid.NewGuid(),
                    Content = "User 2 comment",
                    UserId = user2Id,
                    CreatedAt = DateTime.UtcNow.AddMinutes(-20)
                },
                new Comment
                {
                    Id = Guid.NewGuid(),
                    Content = "User 3 comment",
                    UserId = user3Id,
                    CreatedAt = DateTime.UtcNow.AddMinutes(-10),
                    ParentCommentId = null
                }
            };

        var reply1 = new Comment
        {
            Id = Guid.NewGuid(),
            Content = "User 1 reply",
            UserId = user1Id,
            CreatedAt = DateTime.UtcNow.AddMinutes(-5),
            ParentCommentId = comments[1].Id
        };

        var reply2 = new Comment
        {
            Id = Guid.NewGuid(),
            Content = "User 2 reply",
            UserId = user2Id,
            CreatedAt = DateTime.UtcNow.AddMinutes(-3),
            ParentCommentId = comments[0].Id
        };

        var reply3 = new Comment
        {
            Id = Guid.NewGuid(),
            Content = "User 3 reply",
            UserId = user3Id,
            CreatedAt = DateTime.UtcNow.AddMinutes(-2),
            ParentCommentId = reply1.Id
        };

        comments.Add(reply1);
        comments.Add(reply2);
        comments.Add(reply3);

        context.Comments.AddRange(comments);
        context.SaveChanges();
    }
}
