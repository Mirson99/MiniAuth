using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniAuth.Domain.Entities;

namespace MiniAuth.Infrastructure.Data.Configurations;

public class RolePermissionConfiguration: IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });
        builder.HasData(
            new RolePermission 
            { 
                RoleId = 1, 
                PermissionId = (int)Domain.Enums.Permission.ReadMember 
            },
            new RolePermission 
            { 
                RoleId = 1, 
                PermissionId = (int)Domain.Enums.Permission.UpdateMember 
            }
        );
    }
}       