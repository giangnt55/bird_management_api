﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppCore.Data;

public class BaseEntity
{
    public Guid Id { get; set; }
    public Guid? CreatorId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public class BaseConfig : IEntityTypeConfiguration<BaseEntity>
{
    public void Configure(EntityTypeBuilder<BaseEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasDefaultValueSql("newid()").IsRequired();
        builder.Property(x => x.CreatorId).IsRequired(false);
        builder.Property(x => x.CreatedAt).HasColumnType("datetime");
        builder.Property(x => x.UpdatedAt).HasColumnType("datetime");
        builder.Property(x => x.DeletedAt).HasColumnType("datetime").IsRequired(false);
    }
}