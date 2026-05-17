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
    public class SettlementConfiguration : IEntityTypeConfiguration<Settlement>
    {
        public void Configure(EntityTypeBuilder<Settlement> builder)
        {
            builder.ToTable("Settlements");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Amount)
                .HasPrecision(18, 2);

            builder.Property(s => s.Status)
                .HasConversion<string>();

            // From User (payer)
            builder.HasOne(s => s.FromUser)
                .WithMany(u => u.SentSettlements)
                .HasForeignKey(s => s.FromUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // To User (receiver)
            builder.HasOne(s => s.ToUser)
                .WithMany(u => u.ReceivedSettlements)
                .HasForeignKey(s => s.ToUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
