using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PXGo.Study.Domain.AggregatesModel.Enums;
using PXGo.Study.Domain.AggregatesModel.MessageAggregate;

namespace PXGo.Study.Infrastructure.EntityConfigs;

public class MessageEntityConfigs : IEntityTypeConfiguration<MessageEntity>
{
    public void Configure(EntityTypeBuilder<MessageEntity> config)
    {
        config.ToTable("messages", DBContext.DEFAULT_SCHEMA);
        config.HasKey(t => t.Id);
        config.Ignore(t => t.DomainEvents);

        config.Property(t => t.TypeOne).HasColumnName("type_one");
        config.Property(t => t.TypeTwo).HasColumnName("type_two").HasConversion(new ValueConverter<TwoType, int>(from => from.Id, to => TwoType.From(to)));
        config.Property(t => t.Content).HasColumnName("content").IsRequired(false);
        config.Property(t => t.CreateTime).HasColumnName("create_time");
    }
}
