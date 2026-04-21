using Azure;
using Azure.AI.TextAnalytics;
using ChatApp.Application.Interfaces.Repositories;
using ChatApp.Application.Interfaces.Services;
using ChatApp.Application.UseCases.Messages.SendMessage; // Updated namespace
using ChatApp.Application.Validators; // Updated namespace
using ChatApp.Infrastructure.Data;
using ChatApp.Infrastructure.Repositories;
using ChatApp.Infrastructure.Services;
using ChatApp.Presentation.Hubs;
using ChatApp.Presentation.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 1. Configure Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration)
        .WriteTo.Console()); // <-- This forces logs to the Rider terminal!
// 2. Add API Controllers & Azure SignalR (Switched from Razor Pages to Web API)
builder.Services.AddControllers();
builder.Services.AddSignalR()
    .AddAzureSignalR(builder.Configuration["Azure:SignalR:ConnectionString"]);

// 3. Configure Entity Framework Core with Azure SQL
builder.Services.AddDbContext<ChatAppContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 4. Configure Azure Cognitive Services Singleton
builder.Services.AddSingleton(x =>
{
    var endpoint = builder.Configuration["Azure:TextAnalytics:Endpoint"]!;
    var apiKey = builder.Configuration["Azure:TextAnalytics:ApiKey"]!;
    return new TextAnalyticsClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
});

// 5. Register Application Layer Dependencies (MediatR & FluentValidation)
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<SendMessageCommand>());
builder.Services.AddValidatorsFromAssemblyContaining<SendMessageCommandValidator>();

// 6. Register Repositories and Services
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IChatRoomRepository, ChatRoomRepository>(); // Added new repository
builder.Services.AddSingleton<ISentimentService, SentimentService>();
builder.Services.AddTransient<ISignalRNotifier, SignalRNotifier>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ChatApp.Infrastructure.Data.ChatAppContext>();
    var testUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    
    // Check if the user exists. If not, create them!
    if (!dbContext.Users.Any(u => u.Id == testUserId))
    {
        dbContext.Users.Add(new ChatApp.Domain.Entities.User 
        { 
            Id = testUserId, 
            Username = "TestUser" 
        });
        dbContext.SaveChanges();
    }
}

// 7. Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Serve files from the wwwroot folder (where Angular automatically builds to)
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

// 8. Map Endpoints
app.MapControllers();
app.MapHub<ChatHub>("/chatHub");

// 9. SPA Fallback Routing
// If a route isn't recognized as an API or file, let Angular's router handle it
app.MapFallbackToFile("index.html");

app.Run();