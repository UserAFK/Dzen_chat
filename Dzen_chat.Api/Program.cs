using Application;
using Application.Dto;
using Dzen_chat.Api;
using Dzen_chat.Api.Extentions;
using Infrastructure;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    //.WriteTo.MSSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    //    sinkOptions: new Serilog.Sinks.MSSqlServer.MSSqlServerSinkOptions
    //    {
    //        TableName = "Logs",
    //        AutoCreateSqlTable = true
    //    })
    .CreateLogger();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(c => c.CreateMap<Comment, CommentDto>().ReverseMap());
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalPolicy",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200")
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials();
        });
});
builder.Services.AddResponseCaching();
builder.Services.AddScoped<CommentService>();
builder.Services.AddScoped<FileService>();
builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
builder.Services.AddHostedService<FileProcessingService>();
builder.Services.AddDbContext<IAppDbContext, AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    //options.UseInMemoryDatabase("DzenChatDb"));
builder.Host.UseSerilog();

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
app.CreateDatabase();
app.Run();
