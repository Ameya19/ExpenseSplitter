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
    public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
    {
        public void Configure(EntityTypeBuilder<Expense> builder)
        {
            builder.ToTable("Expenses");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(100);

            // Decimal precision for money
            builder.Property(e => e.Amount)
                .HasPrecision(18, 2);

            // Store enum as string in DB
            builder.Property(e => e.Category)
                .HasConversion<string>();

            builder.Property(e => e.SplitType)
                .HasConversion<string>();

            // Expense paid by User
            builder.HasOne(e => e.PaidBy)
                .WithMany(u => u.PaidExpenses)
                .HasForeignKey(e => e.PaidByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Expense has many Splits
            builder.HasMany(e => e.Splits)
                .WithOne(s => s.Expense)
                .HasForeignKey(s => s.ExpenseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
