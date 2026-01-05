namespace MiniAuth.Application.Common.Interfaces;

public interface IPermissionRepository
{
    Task<HashSet<string>> GetPermissionsAsync(Guid userId);
}