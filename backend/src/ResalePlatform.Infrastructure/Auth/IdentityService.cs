using Microsoft.AspNetCore.Identity;
using ResalePlatform.Application.Common.Interfaces;
using ResalePlatform.Application.Common.Models;
using ResalePlatform.Infrastructure.Identity;

namespace ResalePlatform.Infrastructure.Auth;

public class IdentityService : IIdentityService
{
    public const string DefaultRole = "User";

    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<(bool Succeeded, IEnumerable<string> Errors, Guid UserId)> CreateUserAsync(
        string email, string password, string displayName)
    {
        var existing = await _userManager.FindByEmailAsync(email);
        if (existing is not null)
            return (false, new[] { "Пользователь с таким email уже существует." }, Guid.Empty);

        var user = new ApplicationUser
        {
            Email = email,
            UserName = email,
            DisplayName = displayName,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            return (false, result.Errors.Select(e => e.Description), Guid.Empty);

        await _userManager.AddToRoleAsync(user, DefaultRole);
        return (true, Array.Empty<string>(), user.Id);
    }

    public async Task<AppUserInfo?> ValidateCredentialsAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null || user.IsBlocked)
            return null;

        if (!await _userManager.CheckPasswordAsync(user, password))
            return null;

        return await ToUserInfoAsync(user);
    }

    public async Task<AppUserInfo?> GetUserByIdAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        return user is null ? null : await ToUserInfoAsync(user);
    }

    private async Task<AppUserInfo> ToUserInfoAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        return new AppUserInfo(
            user.Id,
            user.Email!,
            user.DisplayName,
            user.City,
            user.AvatarUrl,
            roles.ToList());
    }
}
