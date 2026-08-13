using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace ResalePlatform.IntegrationTests;

public class FavoritesTests : IntegrationTestBase
{
    public FavoritesTests(ApiFactory factory) : base(factory) { }

    [Fact]
    public async Task Add_favorite_marks_listing_and_appears_in_list()
    {
        var seller = await RegisterAsync();
        var listingId = await CreateListingAsync(AuthedClient(seller.AccessToken));

        var buyer = await RegisterAsync();
        var client = AuthedClient(buyer.AccessToken);

        var add = await client.PostAsync($"/api/favorites/{listingId}", null);
        add.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var favorites = await client.GetFromJsonAsync<List<ListingModel>>("/api/favorites", Json);
        favorites!.Should().ContainSingle(f => f.Id == listingId);

        var listing = await client.GetFromJsonAsync<ListingModel>($"/api/listings/{listingId}", Json);
        listing!.IsFavorite.Should().BeTrue();
    }

    [Fact]
    public async Task Remove_favorite_clears_it()
    {
        var seller = await RegisterAsync();
        var listingId = await CreateListingAsync(AuthedClient(seller.AccessToken));

        var buyer = await RegisterAsync();
        var client = AuthedClient(buyer.AccessToken);
        await client.PostAsync($"/api/favorites/{listingId}", null);

        var del = await client.DeleteAsync($"/api/favorites/{listingId}");
        del.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var favorites = await client.GetFromJsonAsync<List<ListingModel>>("/api/favorites", Json);
        favorites!.Should().NotContain(f => f.Id == listingId);
    }

    [Fact]
    public async Task Favorites_without_auth_returns_401()
    {
        var res = await CreateClient().GetAsync("/api/favorites");
        res.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
