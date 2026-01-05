using MiniAuth.Domain.Entities;

namespace MiniAuth.Application.Common.Interfaces;

public interface ITokenService
{
    Task<string> CreateToken(User user);
    string GenerateRefreshToken();
}