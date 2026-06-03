using ExpenseSplitter.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseSplitter.Infrastructure.Configurations
{
    public class ExpenseSplitConfiguration : IEntityTypeConfiguration<ExpenseSplit>
    {
        public void Configure(EntityTypeBuilder<ExpenseSplit> builder)
        {
            builder.ToTable("ExpenseSplits");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.ShareAmount)
                .HasPrecision(18, 2);

            builder.Property(s => s.SharePercentage)
                .HasPrecision(5, 2);   // e.g. 33.33%

            // Split belongs to User
            builder.HasOne(s => s.User)
                .WithMany(u => u.ExpenseSplits)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Split belongs to Expense
            builder.HasOne(s => s.Expense)
                .WithMany(e => e.Splits)
                .HasForeignKey(s => s.ExpenseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
