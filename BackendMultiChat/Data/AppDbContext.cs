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
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<FileSaveInServer> FileSaveInServers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite key for GroupMember
            modelBuilder.Entity<GroupMember>()
                .HasKey(gm => new { gm.ContactId, gm.ConversationId });

            // Define relationships (Message <-> Conversation)
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId);

            // Define relationships (GroupMember <-> Conversation <-> Contact)
            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.Conversation)
                .WithMany(c => c.GroupMembers)
                .HasForeignKey(gm => gm.ConversationId);

            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.Contact)
                .WithMany(c => c.GroupMembers)
                .HasForeignKey(gm => gm.ContactId);

            modelBuilder.Entity<FileSaveInServer>()
               .HasKey(fs => fs.FileId);

            modelBuilder.Entity<FileSaveInServer>()
                .HasOne(fs => fs.Conversation)       
                .WithMany(c => c.Files)             
                .HasForeignKey(fs => fs.ConversationID);
        }
    }
}
