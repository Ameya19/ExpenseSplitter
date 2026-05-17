using ExpenseSplitter.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Group = ExpenseSplitter.Core.Entities.Group;

namespace ExpenseSplitter.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
         : base(options) { }

        // Each DbSet = One Table
        public DbSet<Core.Entities.Group> Groups => Set<Group>();
        public DbSet<GroupMember> GroupMembers => Set<GroupMember>();
        public DbSet<Expense> Expenses => Set<Expense>();
        public DbSet<ExpenseSplit> ExpenseSplits => Set<ExpenseSplit>();
        public DbSet<Settlement> Settlements => Set<Settlement>();
        public DbSet<Notification> Notifications => Set<Notification>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Auto-apply all configurations from this assembly
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(AppDbContext).Assembly);

            // Global soft delete filter
            modelBuilder.Entity<Group>()
                .HasQueryFilter(g => !g.IsDeleted);
            modelBuilder.Entity<Expense>()
                .HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
