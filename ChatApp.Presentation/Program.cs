using System.Text;
using Azure;
using Azure.AI.TextAnalytics;
using ChatApp.Application.Interfaces.Repositories;
using ChatApp.Application.Interfaces.Services;
using ChatApp.Infrastructure.Data;
using ChatApp.Infrastructure.Repositories;
using ChatApp.Infrastructure.Services;
using ChatApp.Presentation.Hubs;
using ChatApp.Presentation.Middleware;
using ChatApp.Presentation.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration).WriteTo.Console());

builder.Services.AddControllers();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chatHub"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddSignalR().AddAzureSignalR(builder.Configuration["Azure:SignalR:ConnectionString"]);

builder.Services.AddDbContext<ChatAppContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton(x =>
{
    var endpoint = builder.Configuration["Azure:TextAnalytics:Endpoint"]!;
    var apiKey = builder.Configuration["Azure:TextAnalytics:ApiKey"]!;
    return new TextAnalyticsClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
});

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ChatApp.Application.UseCases.Messages.SendMessage.SendMessageCommand).Assembly));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();

builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IChatRoomRepository, ChatRoomRepository>();
builder.Services.AddSingleton<ISentimentService, SentimentService>();
builder.Services.AddTransient<ISignalRNotifier, SignalRNotifier>();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chatHub");
app.MapFallbackToFile("index.html");

app.Run();