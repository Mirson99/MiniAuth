using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniAuth.Domain.Entities;

namespace MiniAuth.Infrastructure.Data.Configurations;

public class RoleConfiguration: IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasMany(x => x.Permissions)
            .WithMany().UsingEntity<RolePermission>();
        
        IEnumerable<Role> roles = Enum.GetValues<Domain.Enums.Role>()
            .Select(p => new Role
            {
                Id = (int)p,
                Name = p.ToString()
            });
        
        builder.HasData(roles);
    }
}