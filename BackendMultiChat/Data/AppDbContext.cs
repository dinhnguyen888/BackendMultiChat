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

        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectMember> ProjectMembers { get; set; }
        public DbSet<TodoItem> TodoItems { get; set; }
        public DbSet<TodoList> TodoLists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ensure Email is unique for each account
            modelBuilder.Entity<Account>()
                .HasIndex(a => a.Email)
                .IsUnique();

            // One-to-One: Each Account can have one RefreshToken
            modelBuilder.Entity<Account>()
                .HasOne(a => a.RefreshToken)
                .WithOne(rt => rt.Account)
                .HasForeignKey<RefreshToken>(rt => rt.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: An Account can have many GroupMembers
            modelBuilder.Entity<Account>()
                .HasMany(a => a.GroupMembers)
                .WithOne(gm => gm.Account)
                .HasForeignKey(gm => gm.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            // Composite key for GroupMember
            modelBuilder.Entity<GroupMember>()
                .HasKey(gm => new { gm.AccountId, gm.RoomId });

            // One-to-Many: A GroupMember belongs to a Room
            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.Rooms)
                .WithMany(c => c.GroupMembers)
                .HasForeignKey(gm => gm.RoomId);

            // One-to-Many: A GroupMember belongs to an Account
            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.Account)
                .WithMany(c => c.GroupMembers)
                .HasForeignKey(gm => gm.AccountId);

            // Primary Key for FileStorage
            modelBuilder.Entity<FileStorage>()
                .HasKey(fs => fs.FileId);

            // One-to-Many: A FileStorage belongs to a Room
            modelBuilder.Entity<FileStorage>()
                .HasOne(fs => fs.Rooms)
                .WithMany(c => c.FileStorages)
                .HasForeignKey(fs => fs.RoomId);

            // One-to-Many: A Room can have many GroupMembers
            modelBuilder.Entity<Room>()
                .HasMany(c => c.GroupMembers)
                .WithOne(gm => gm.Rooms)
                .HasForeignKey(gm => gm.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: A Room can have many FileStorages
            modelBuilder.Entity<Room>()
                .HasMany(c => c.FileStorages)
                .WithOne(fs => fs.Rooms)
                .HasForeignKey(fs => fs.RoomId);

            // One-to-Many: A Room can have many Messages
            modelBuilder.Entity<Room>()
                .HasMany(c => c.Messages)
                .WithOne(m => m.Rooms)
                .HasForeignKey(m => m.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            // Define RoomId as manually assigned (not auto-generated)
            modelBuilder.Entity<Room>()
                .Property(r => r.RoomId)
                .ValueGeneratedNever();

            // Primary Key for Project
            modelBuilder.Entity<Project>()
                .HasKey(p => p.ProjectId);

            // One-to-Many: A Project belongs to an Account (Owner)
            modelBuilder.Entity<Project>()
                .HasOne(p => p.Owner)
                .WithMany(a => a.Projects)
                .HasForeignKey(p => p.OwnerId);

            // One-to-Many: A Project can have many ProjectMembers
            // BO SUNG TINH NANG XOA PROJECT THI XOA PROJECT MEMBER TRONG SERVICE
            modelBuilder.Entity<Project>()
                .HasMany(p => p.ProjectMembers)
                .WithOne(pm => pm.Project)
                .HasForeignKey(pm => pm.ProjectId)
                .OnDelete(DeleteBehavior.NoAction);

            // One-to-Many: A Project can have many TodoLists
            modelBuilder.Entity<Project>()
                .HasMany(p => p.TodoLists)
                .WithOne(tl => tl.Project)
                .HasForeignKey(tl => tl.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // Composite key for ProjectMember
            modelBuilder.Entity<ProjectMember>()
                .HasKey(pm => new { pm.ProjectId, pm.AccountId });

            // One-to-Many: A ProjectMember belongs to a Project
            modelBuilder.Entity<ProjectMember>()
                .HasOne(pm => pm.Project)
                .WithMany(p => p.ProjectMembers)
                .HasForeignKey(pm => pm.ProjectId);

            // One-to-Many: A ProjectMember belongs to an Account
            modelBuilder.Entity<ProjectMember>()
                .HasOne(pm => pm.Account)
                .WithMany(a => a.ProjectMember)
                .HasForeignKey(pm => pm.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
            

            // Primary Key for TodoList
            modelBuilder.Entity<TodoList>()
                .HasKey(tl => tl.TodoListId);

            modelBuilder.Entity<TodoList>()
                .HasOne(tl => tl.Account)
                .WithMany(a => a.TodoLists)
                .HasForeignKey(tl => tl.UserId);

            // One-to-Many: A TodoList belongs to a Project
            // VIET THEM TINH NANG XOA PROJECT THI XOA TODO LIST TRONG SERVICE
            modelBuilder.Entity<TodoList>()
                .HasOne(tl => tl.Project)
                .WithMany(p => p.TodoLists)
                .HasForeignKey(tl => tl.ProjectId)
                .OnDelete(DeleteBehavior.NoAction);
            

            // One-to-Many: A TodoItem belongs to a TodoList
            modelBuilder.Entity<TodoItem>()
                .HasOne(ti => ti.TodoList)
                .WithMany(tl => tl.TodoItems)
                .HasForeignKey(ti => ti.TodoListId)
                .OnDelete(DeleteBehavior.Cascade);
        }


    }
}
