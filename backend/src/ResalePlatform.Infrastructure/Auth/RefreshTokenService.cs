using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ResalePlatform.Application.Common.Interfaces;
using ResalePlatform.Infrastructure.Identity;
using ResalePlatform.Infrastructure.Persistence;

namespace ResalePlatform.Infrastructure.Auth;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly AppDbContext _db;
    private readonly JwtSettings _settings;

    public RefreshTokenService(AppDbContext db, IOptions<JwtSettings> settings)
    {
        _db = db;
        _settings = settings.Value;
    }

    public async Task<string> IssueAsync(Guid userId, CancellationToken ct = default)
    {
        var token = GenerateSecureToken();

        _db.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            CreatedAt = DateTimeOffset.UtcNow,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(_settings.RefreshTokenDays),
        });

        await _db.SaveChangesAsync(ct);
        return token;
    }

    public async Task<Guid?> ConsumeAsync(string token, CancellationToken ct = default)
    {
        var entity = await _db.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == token, ct);

        if (entity is null || !entity.IsActive)
            return null;

        // Ротация: гасим текущий токен, новый выдаётся вызывающим кодом.
        entity.RevokedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(ct);

        return entity.UserId;
    }

    public async Task RevokeAllAsync(Guid userId, CancellationToken ct = default)
    {
        await _db.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.RevokedAt, DateTimeOffset.UtcNow), ct);
    }

    private static string GenerateSecureToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }
}
