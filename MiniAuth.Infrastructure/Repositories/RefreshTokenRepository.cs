using Microsoft.EntityFrameworkCore;
using MiniAuth.Application.Common.Interfaces;
using MiniAuth.Domain.Entities;
using MiniAuth.Infrastructure.Data;

namespace MiniAuth.Infrastructure.Repositories;

public class RefreshTokenRepository: IRefreshTokenRepository
{
    private AuthDbContext _authDbContext;

    public RefreshTokenRepository(AuthDbContext authDbContext)
    {
        _authDbContext = authDbContext;
    }
    
    public async void CreateNewRefreshTokenAsync(RefreshToken token)
    {
        await _authDbContext.RefreshTokens.AddAsync(token);
        await _authDbContext.SaveChangesAsync();
    }

    public async Task<RefreshToken> GetRefreshTokenAsync(string refreshToken)
    {
        return await _authDbContext.RefreshTokens.Include(t => t.User).FirstOrDefaultAsync(t => t.Token == refreshToken);
    }

    public async Task UpdateRefreshTokenAsync(RefreshToken token)
    {
        _authDbContext.RefreshTokens.Update(token);
        await _authDbContext.SaveChangesAsync();
    }

    public async Task RevokeRefreshTokensForUserAsync(Guid userId)
    { 
        await _authDbContext.RefreshTokens.Where(t => t.UserId == userId).ExecuteDeleteAsync();
    }
    
    public async Task AddRefreshTokenAsync(RefreshToken token)
    { 
        await _authDbContext.RefreshTokens.AddAsync(token);
    }
}