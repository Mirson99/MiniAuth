using Microsoft.AspNetCore.Authorization;
using MiniAuth.Domain.Enums;

namespace MiniAuth.Infrastructure.Authorization;

public sealed class HasPermissionAttribute: AuthorizeAttribute
{
    public HasPermissionAttribute(Permission permission)
        : base(policy: permission.ToString())
    {
        
    }
}