using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Species : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string CommonName { get; set; } = string.Empty;
    public string ScientificName { get; set; } = string.Empty;
    public string Description  { get; set; } = string.Empty;
    public ConservationType Conservation  { get; set; }
    
    // Relationship
    public IEnumerable<Bird> Birds { get; set; } = new List<Bird>();
}

public enum ConservationType
{
    Endangered = 1, Vulnerable = 2, LeastConcern = 3
}

public class SpeciesConfig : IEntityTypeConfiguration<Species>
{
    public void Configure(EntityTypeBuilder<Species> builder)
    {
        builder.Property(x => x.Name).IsRequired(false);
        builder.Property(x => x.CommonName).IsRequired(false);
        builder.Property(x => x.ScientificName).IsRequired();
        builder.Property(x => x.Description).IsRequired(false);
        builder.Property(x => x.Conservation).IsRequired();
        builder.HasMany(x => x.Birds);
    }
}