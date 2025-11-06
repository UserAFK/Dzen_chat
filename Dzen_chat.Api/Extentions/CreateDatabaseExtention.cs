using Application;
using Infrastructure;

namespace Dzen_chat.Api.Extentions
{
    public static class CreateDatabaseExtention
    {
        public static void CreateDatabase(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<AppDbContext>();

                try
                {
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();
                    DbSeeder.Seed(context);
                    app.Logger.LogInformation("Database checked and ready.");
                }
                catch (Exception ex)
                {
                    app.Logger.LogError(ex, "An error occurred while initializing the database.");
                }
            }
        }

    }
}
