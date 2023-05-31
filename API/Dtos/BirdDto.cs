using AppCore.Models;
using MainData.Entities;

namespace API.Dtos;

public class BirdDto : BaseDto
{
    public string Name { get; set; } = string.Empty;
    public string? Image { get; set; }
    public string? Description { get; set; }
    public string? Habitat { get; set; }
    public float? AvgLifeSpan { get; set; }
    public float? AvgLifeSize { get; set; }
    public Conservation Conservation { get; set; }
}

public class BirdDetailDto : BaseDto
{
    public string Name { get; set; } = string.Empty;
    public string? Image { get; set; }
    public string? Description { get; set; }
    public string? Habitat { get; set; }
    public float? AvgLifeSpan { get; set; }
    public float? AvgLifeSize { get; set; }
    public Conservation Conservation { get; set; }
    

}

public class BirdQueryDto : BaseQueryDto
{
    public string? BirdName { get; set; }
}

public class BirdUpdateDto : BaseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Image { get; set; }
    public string? Description { get; set; }
    public string? Habitat { get; set; }
    public float? AvgLifeSpan { get; set; }
    public float? AvgLifeSize { get; set; }
    public Conservation Conservation { get; set; }
}

public class BirdCreateDto : BaseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Image { get; set; }
    public string? Description { get; set; }
    public string? Habitat { get; set; }
    public float? AvgLifeSpan { get; set; }
    public float? AvgLifeSize { get; set; }
    public Conservation Conservation { get; set; }
}

public class BirdDeleteDto : BaseDto
{
    public Guid Id { get; set; }
}