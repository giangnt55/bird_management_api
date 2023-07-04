using AppCore.Data;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Follower : BaseEntity
{
  public Guid FollowTo { get; set; }

  //Relationship
  public virtual User User { get; set; }
}

public class FollowerConfig : IEntityTypeConfiguration<Follower>
{
  public void Configure(EntityTypeBuilder<Follower> builder)
  {
    builder.Property(x => x.FollowTo).IsRequired();

    //Relationship
    builder.HasOne(x => x.User)
      .WithMany(x => x.Followers)
      .HasForeignKey(x => x.FollowTo);

  }
}
