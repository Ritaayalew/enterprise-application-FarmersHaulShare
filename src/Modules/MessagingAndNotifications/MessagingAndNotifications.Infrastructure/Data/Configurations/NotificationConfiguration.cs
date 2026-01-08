using MessagingAndNotifications.Domain.Entities;
using MessagingAndNotifications.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MessagingAndNotifications.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for Notification entity
/// </summary>
public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(n => n.Id);

        builder.Property(n => n.RecipientId)
            .IsRequired();

        builder.Property(n => n.RecipientType)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(n => n.NotificationType)
            .HasMaxLength(50)
            .IsRequired();

        // Configure NotificationChannel value object
        builder.OwnsOne(n => n.Channel, channelBuilder =>
        {
            channelBuilder.Property(c => c.ChannelType)
                .HasColumnName("ChannelType")
                .HasMaxLength(20)
                .IsRequired();

            channelBuilder.Property(c => c.Address)
                .HasColumnName("ChannelAddress")
                .HasMaxLength(255);
        });

        // Configure NotificationTemplate value object
        builder.OwnsOne(n => n.Template, templateBuilder =>
        {
            templateBuilder.Property(t => t.TemplateName)
                .HasColumnName("TemplateName")
                .HasMaxLength(100)
                .IsRequired();

            templateBuilder.Property(t => t.Subject)
                .HasColumnName("TemplateSubject")
                .HasMaxLength(500)
                .IsRequired();

            templateBuilder.Property(t => t.Body)
                .HasColumnName("TemplateBody")
                .HasColumnType("nvarchar(max)")
                .IsRequired();

            templateBuilder.Property(t => t.LanguageCode)
                .HasColumnName("TemplateLanguageCode")
                .HasMaxLength(10);
        });

        // Configure NotificationStatus value object
        builder.OwnsOne(n => n.Status, statusBuilder =>
        {
            statusBuilder.Property(s => s.Status)
                .HasColumnName("Status")
                .HasMaxLength(20)
                .IsRequired();

            statusBuilder.Property(s => s.StatusChangedAtUtc)
                .HasColumnName("StatusChangedAtUtc");

            statusBuilder.Property(s => s.FailureReason)
                .HasColumnName("FailureReason")
                .HasMaxLength(500);
        });

        builder.Property(n => n.Subject)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(n => n.Body)
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        builder.Property(n => n.RelatedEntityId);

        builder.Property(n => n.RelatedEntityType)
            .HasMaxLength(50);

        // Store Metadata as JSON
        builder.Property(n => n.Metadata)
            .HasConversion(
                v => v == null ? null : System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => v == null ? null : System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions?)null))
            .HasColumnType("nvarchar(max)");

        builder.Property(n => n.CreatedAtUtc)
            .IsRequired();

        builder.Property(n => n.SentAtUtc);

        builder.Property(n => n.DeliveredAtUtc);

        // Indexes
        builder.HasIndex(n => n.RecipientId);
        builder.HasIndex(n => n.NotificationType);
        builder.HasIndex(n => new { n.RelatedEntityId, n.RelatedEntityType });
        builder.HasIndex(n => n.CreatedAtUtc);
    }
}
