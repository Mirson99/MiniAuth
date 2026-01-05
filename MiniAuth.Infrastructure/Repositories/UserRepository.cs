using Microsoft.EntityFrameworkCore;
using MiniAuth.Application.Common.Interfaces;
using MiniAuth.Domain.Entities;
using MiniAuth.Infrastructure.Data;

namespace MiniAuth.Infrastructure.Repositories;

public class UserRepository: IUserRepository
{
    private readonly AuthDbContext _authDbContext;
    public UserRepository(AuthDbContext authDbContext)
    {
        _authDbContext = authDbContext;
    }
    
    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _authDbContext.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        await _authDbContext.Users.AddAsync(user);
        await _authDbContext.SaveChangesAsync();
        return user;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _authDbContext.Users.FirstOrDefaultAsync(u => u.Email == email.ToLower());
    }
}