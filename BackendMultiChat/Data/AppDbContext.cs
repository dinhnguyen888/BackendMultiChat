using BackendMultiChat.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BackendMultiChat.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Message> Messages { get; set; }
      
        public DbSet<Room> Rooms { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<FileStorage> FileStorages { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>()
                .HasIndex(a => a.Email)
                .IsUnique();

            modelBuilder.Entity<Account>()
                .HasOne(a => a.RefreshToken)
                .WithOne(rt => rt.Account)
                .HasForeignKey<RefreshToken>(rt => rt.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.GroupMembers)
                .WithOne(gm => gm.Account)
                .HasForeignKey(gm => gm.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            // Composite key for GroupMember
            modelBuilder.Entity<GroupMember>()
                .HasKey(gm => new { gm.AccountId, gm.RoomId });

            // Define relationships (Message <-> Conversation)
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId);

            // Define relationships (GroupMember <-> Conversation <-> Contact)
            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.Rooms)
                .WithMany(c => c.GroupMembers)
                .HasForeignKey(gm => gm.RoomId);

            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.Account)
                .WithMany(c => c.GroupMembers)
                .HasForeignKey(gm => gm.AccountId);

            modelBuilder.Entity<FileStorage>()
               .HasKey(fs => fs.FileId);

            modelBuilder.Entity<FileStorage>()
                .HasOne(fs => fs.Rooms)       
                .WithMany(c => c.Files)             
                .HasForeignKey(fs => fs.RoomId);

            modelBuilder.Entity<Room>()
                .HasMany(c => c.GroupMembers)
                .WithOne(gm => gm.Rooms)
                .HasForeignKey(gm => gm.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
