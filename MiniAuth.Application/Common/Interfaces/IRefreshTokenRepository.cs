using MiniAuth.Domain.Entities;

namespace MiniAuth.Application.Common.Interfaces;

public interface IRefreshTokenRepository
{
    public void CreateNewRefreshTokenAsync(RefreshToken token);
    public Task<RefreshToken> GetRefreshTokenAsync(string refreshToken);
    public Task UpdateRefreshTokenAsync(RefreshToken token);
    public Task RevokeRefreshTokensForUserAsync(Guid userId);
    public Task AddRefreshTokenAsync(RefreshToken token);

}