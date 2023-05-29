using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Bird : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Image { get; set; }
    public string? Description { get; set; }
    public string? Habitat { get; set; }
    public float? AvgLifeSpan { get; set; }
    public float? AvgLifeSize { get; set; }
    public Conservation Conservation { get; set; }
}

public enum Conservation
{
    NotEvaluated = 1, DataDeficient = 2, LeastConcern = 3, NearThreatened = 4,
    Vulnerable = 5, Endangered = 6, CriticallyEndangered = 7, ExtinctInTheWild = 8,
    Extinct = 9
}

public class BirdConfig : IEntityTypeConfiguration<Bird>
{
    public void Configure(EntityTypeBuilder<Bird> builder)
    {
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.Image).IsRequired(false);
        builder.Property(x => x.Description).IsRequired(false);
        builder.Property(x => x.Habitat).IsRequired(false);
        builder.Property(x => x.AvgLifeSize).IsRequired(false);
        builder.Property(x => x.AvgLifeSpan).IsRequired(false);
        builder.Property(x => x.Conservation).IsRequired();
    }
}