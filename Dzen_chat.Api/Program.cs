using Application;
using Application.misc;
using Dzen_chat.Api;
using Dzen_chat.Api.Services;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalPolicy",
        builder =>
        {
            builder.WithOrigins(
                "http://localhost:4200",
                "http://dzenchat-web",
                "http://frontend")
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials();
        });
});
builder.Services.AddResponseCaching();
builder.Services.AddHttpClient();
builder.Services.AddScoped<CaptchaService>();
builder.Services.AddScoped<CommentService>();
builder.Services.AddScoped<FileService>();
builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
builder.Services.AddHostedService<FileProcessingService>();
builder.Services.AddDbContext<IAppDbContext, AppDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Host.UseSerilog();
builder.Services.Configure<RecaptchaSettings>(
    builder.Configuration.GetSection("Recaptcha"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHsts();

app.UseHttpsRedirection();
app.UseResponseCaching();
app.UseCors("LocalPolicy");

app.UseAuthorization();

app.MapControllers();
app.MapHub<CommentHub>("/commentHub")
    .RequireCors("LocalPolicy");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}
app.Run();
