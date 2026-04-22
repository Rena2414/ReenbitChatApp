using ChatApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Data;

public class ChatAppContext : DbContext
{
    public ChatAppContext(DbContextOptions<ChatAppContext> options) : base(options) { }

    public DbSet<Message> Messages { get; set; } = null!;
    public DbSet<ChatRoom> ChatRooms { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.PasswordHash).IsRequired(); 
        });

        modelBuilder.Entity<ChatRoom>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).IsRequired().HasMaxLength(500);
            
            entity.Property(e => e.Sentiment).HasConversion<string>(); 

            entity.HasOne(m => m.User)
                .WithMany(u => u.Messages)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(m => m.ChatRoom)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChatRoomId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}