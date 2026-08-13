using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace ResalePlatform.IntegrationTests;

public class ListingsTests : IntegrationTestBase
{
    public ListingsTests(ApiFactory factory) : base(factory) { }

    [Fact]
    public async Task Create_without_auth_returns_401()
    {
        var categoryId = await FirstLeafCategoryIdAsync();
        var res = await CreateClient().PostAsJsonAsync("/api/listings", new
        {
            title = "X", description = "d", price = 1, condition = "New", city = "М", categoryId,
        });

        res.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_then_get_returns_listing()
    {
        var auth = await RegisterAsync(displayName: "Продавец");
        var client = AuthedClient(auth.AccessToken);
        var id = await CreateListingAsync(client, "iPhone", 25000);

        var listing = await CreateClient().GetFromJsonAsync<ListingModel>($"/api/listings/{id}", Json);

        listing!.Title.Should().Be("iPhone");
        listing.Price.Should().Be(25000);
        listing.Status.Should().Be("Active");
        listing.SellerName.Should().Be("Продавец");
    }

    [Fact]
    public async Task Update_by_non_owner_returns_403()
    {
        var owner = await RegisterAsync();
        var id = await CreateListingAsync(AuthedClient(owner.AccessToken));
        var categoryId = await FirstLeafCategoryIdAsync();

        var other = await RegisterAsync();
        var res = await AuthedClient(other.AccessToken).PutAsJsonAsync($"/api/listings/{id}", new
        {
            id, title = "Взлом", description = "d", price = 1,
            condition = "New", status = "Active", city = "М", categoryId,
        });

        res.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_by_non_owner_returns_403()
    {
        var owner = await RegisterAsync();
        var id = await CreateListingAsync(AuthedClient(owner.AccessToken));

        var other = await RegisterAsync();
        var res = await AuthedClient(other.AccessToken).DeleteAsync($"/api/listings/{id}");

        res.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_by_owner_succeeds_then_404()
    {
        var owner = await RegisterAsync();
        var client = AuthedClient(owner.AccessToken);
        var id = await CreateListingAsync(client);

        var del = await client.DeleteAsync($"/api/listings/{id}");
        del.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var get = await CreateClient().GetAsync($"/api/listings/{id}");
        get.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
