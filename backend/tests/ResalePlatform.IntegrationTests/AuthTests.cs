using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace ResalePlatform.IntegrationTests;

public class AuthTests : IntegrationTestBase
{
    public AuthTests(ApiFactory factory) : base(factory) { }

    [Fact]
    public async Task Register_returns_token_and_user_with_role()
    {
        var auth = await RegisterAsync(displayName: "Иван");

        auth.AccessToken.Should().NotBeNullOrEmpty();
        auth.RefreshToken.Should().NotBeNullOrEmpty();
        auth.User.DisplayName.Should().Be("Иван");
        auth.User.Roles.Should().Contain("User");
    }

    [Fact]
    public async Task Login_with_valid_credentials_succeeds()
    {
        var email = $"login_{Guid.NewGuid():N}@test.local";
        await RegisterAsync(email);

        var res = await CreateClient().PostAsJsonAsync("/api/auth/login",
            new { email, password = "Passw0rd" });

        res.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Login_with_wrong_password_returns_401()
    {
        var email = $"wrong_{Guid.NewGuid():N}@test.local";
        await RegisterAsync(email);

        var res = await CreateClient().PostAsJsonAsync("/api/auth/login",
            new { email, password = "WRONG" });

        res.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Register_duplicate_email_returns_409()
    {
        var email = $"dup_{Guid.NewGuid():N}@test.local";
        await RegisterAsync(email);

        var res = await CreateClient().PostAsJsonAsync("/api/auth/register",
            new { email, password = "Passw0rd", displayName = "Dup" });

        res.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Register_weak_password_returns_400()
    {
        var res = await CreateClient().PostAsJsonAsync("/api/auth/register",
            new { email = $"weak_{Guid.NewGuid():N}@test.local", password = "12", displayName = "W" });

        res.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Me_without_token_returns_401()
    {
        var res = await CreateClient().GetAsync("/api/auth/me");
        res.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Me_with_token_returns_current_user()
    {
        var auth = await RegisterAsync();

        var me = await AuthedClient(auth.AccessToken)
            .GetFromJsonAsync<UserModel>("/api/auth/me", Json);

        me!.Id.Should().Be(auth.User.Id);
    }
}
