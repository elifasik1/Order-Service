using Domain.Entities;
using OrderService.Application.Interfaces;
using OrderService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace OrderService.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _dbContext;

    public RefreshTokenRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(RefreshToken refreshToken)
    {
        await _dbContext.RefreshTokens.AddAsync(refreshToken);
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _dbContext.RefreshTokens
            .SingleOrDefaultAsync(x => x.Token == token);
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public Task UpdateAsync(RefreshToken refreshToken)
    {
         _dbContext.RefreshTokens.Update(refreshToken);
    return Task.CompletedTask;
    }
}