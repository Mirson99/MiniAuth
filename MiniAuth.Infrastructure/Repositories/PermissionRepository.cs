using Microsoft.EntityFrameworkCore;
using MiniAuth.Application.Common.Interfaces;
using MiniAuth.Domain.Entities;
using MiniAuth.Infrastructure.Data;

namespace MiniAuth.Infrastructure.Repositories;

public class PermissionRepository: IPermissionRepository
{
    private readonly AuthDbContext _context;
    
    public PermissionRepository(AuthDbContext context)
    {
        _context = context;
    }
    
    public async Task<HashSet<string>> GetPermissionsAsync(Guid userId)
    {
        ICollection<Role>[] roles = await _context.Users
            .Include(x => x.Roles)
            .ThenInclude(x => x.Permissions)
            .Where(x => x.Id == userId)
            .Select(x => x.Roles)
            .ToArrayAsync();
        
        return roles
            .SelectMany(x => x)
            .SelectMany(x => x.Permissions)
            .Select(x => x.Name)
            .ToHashSet();
    }
}