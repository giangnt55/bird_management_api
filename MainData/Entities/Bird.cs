using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Bird : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public string Description  { get; set; } = string.Empty;
    public string Habitat { get; set; } = string.Empty;
    public string AvgLifespan { get; set; } = string.Empty;
    public string AvgLifeSize { get; set; } = string.Empty;
    public double AvgPrice { get; set; }
    
    //Relationship
    public Guid SpeciesId { get; set; }
}

public class BirdConfig : IEntityTypeConfiguration<Bird>
{
    public void Configure(EntityTypeBuilder<Bird> builder)
    {
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.Image).IsRequired(false);
        builder.Property(x => x.Description).IsRequired(false);
        builder.Property(x => x.Habitat).IsRequired(false);
        builder.Property(x => x.AvgLifespan).IsRequired(false);
        builder.Property(x => x.AvgLifeSize).IsRequired(false);
        builder.Property(x => x.AvgPrice).IsRequired().HasDefaultValue(0);
        builder.Property(x => x.SpeciesId).IsRequired();
    }
}