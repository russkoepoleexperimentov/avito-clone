using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace ResalePlatform.IntegrationTests;

public class CatalogTests : IntegrationTestBase
{
    public CatalogTests(ApiFactory factory) : base(factory) { }

    [Fact]
    public async Task Catalog_finds_listing_by_unique_search_term()
    {
        var token = Guid.NewGuid().ToString("N");
        var auth = await RegisterAsync();
        var id = await CreateListingAsync(AuthedClient(auth.AccessToken), $"Уникум {token}");

        var page = await CreateClient()
            .GetFromJsonAsync<PagedListings>($"/api/listings?search={token}", Json);

        page!.Items.Should().ContainSingle(i => i.Id == id);
    }

    [Fact]
    public async Task Catalog_filters_by_price_range()
    {
        var term = Guid.NewGuid().ToString("N");
        var auth = await RegisterAsync();
        var client = AuthedClient(auth.AccessToken);
        await CreateListingAsync(client, $"Дешёвый {term}", 100);
        var expensiveId = await CreateListingAsync(client, $"Дорогой {term}", 100000);

        var page = await CreateClient()
            .GetFromJsonAsync<PagedListings>($"/api/listings?search={term}&minPrice=50000", Json);

        page!.Items.Should().ContainSingle()
            .Which.Id.Should().Be(expensiveId);
    }

    [Fact]
    public async Task Catalog_supports_pagination()
    {
        var page = await CreateClient()
            .GetFromJsonAsync<PagedListings>("/api/listings?pageSize=2&page=1", Json);

        page!.PageSize.Should().Be(2);
        page.Items.Count.Should().BeLessThanOrEqualTo(2);
    }
}
