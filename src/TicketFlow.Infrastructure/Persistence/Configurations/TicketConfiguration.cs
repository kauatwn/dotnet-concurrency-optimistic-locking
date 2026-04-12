using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketFlow.Domain.Entities;
using TicketFlow.Domain.ValueObjects;

namespace TicketFlow.Infrastructure.Persistence.Configurations;

public sealed class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .ValueGeneratedNever();

        builder.Property(t => t.Price)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.Status)
            .IsRequired();

        builder.Property(t => t.Version)
            .IsRowVersion();

        builder.OwnsOne(t => t.Seat, seatBuilder =>
        {
            seatBuilder.Property(s => s.Sector)
                .HasMaxLength(Seat.MaxSectorLength)
                .IsRequired();

            seatBuilder.Property(s => s.Row)
                .HasMaxLength(Seat.MaxRowLength)
                .IsRequired();

            seatBuilder.Property(s => s.Number)
                .IsRequired();
        });

        builder.HasOne<Show>()
            .WithMany()
            .HasForeignKey(t => t.ShowId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}