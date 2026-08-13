using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace ResalePlatform.IntegrationTests;

public class CategoriesTests : IntegrationTestBase
{
    public CategoriesTests(ApiFactory factory) : base(factory) { }

    [Fact]
    public async Task Get_categories_returns_seeded_tree()
    {
        var cats = await CreateClient().GetFromJsonAsync<List<CategoryNode>>("/api/categories", Json);

        cats.Should().NotBeNullOrEmpty();
        cats!.Should().Contain(c => c.Slug == "electronics");
        cats!.First(c => c.Slug == "electronics").Children.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Create_category_as_non_admin_returns_403()
    {
        var user = await RegisterAsync();

        var res = await AuthedClient(user.AccessToken).PostAsJsonAsync("/api/categories",
            new { name = "Хак", slug = $"hack-{Guid.NewGuid():N}", parentId = (Guid?)null, sortOrder = 0 });

        res.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Create_category_as_admin_succeeds()
    {
        var admin = await LoginAdminAsync();
        var slug = $"cat-{Guid.NewGuid():N}";

        var res = await AuthedClient(admin.AccessToken).PostAsJsonAsync("/api/categories",
            new { name = "Новая", slug, parentId = (Guid?)null, sortOrder = 50 });

        res.StatusCode.Should().Be(HttpStatusCode.Created);

        var cats = await CreateClient().GetFromJsonAsync<List<CategoryNode>>("/api/categories", Json);
        cats!.Should().Contain(c => c.Slug == slug);
    }

    [Fact]
    public async Task Admin_can_list_users()
    {
        var admin = await LoginAdminAsync();

        var res = await AuthedClient(admin.AccessToken).GetAsync("/api/admin/users");

        res.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
