using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.BL.Model;

namespace TaskManager.BL.Controller
{
    class TaskManagerContext : DbContext
    {
        public DbSet<Board> Boards { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<User> Users { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer("Server = (localdb)\\mssqllocaldb; Database = TaskManagerTestdb; Trusted_Connection = True; ");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BoardConfiguration());
            modelBuilder.ApplyConfiguration(new TaskConfiguration());
        }
    }

    public class BoardConfiguration : IEntityTypeConfiguration<Board>
    {
        public void Configure(EntityTypeBuilder<Board> builder)
        {
            builder.HasAlternateKey(b => b.Name);
            builder.Property(b => b.Name).IsRequired();
        }
    }

    public class TaskConfiguration : IEntityTypeConfiguration<Task>
    {
        public void Configure(EntityTypeBuilder<Task> builder)
        {
            builder.HasAlternateKey(b => b.Name);
            builder.Property(b => b.Name).IsRequired();
            builder.Property(b => b.DeadLine).IsRequired();
            builder.Property(b => b.Priority).IsRequired();
            builder.Ignore(b => b.Period);
        }
    }
}
