using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace ResalePlatform.IntegrationTests;

[Collection("api")]
public abstract class IntegrationTestBase
{
    protected readonly ApiFactory Factory;

    protected static readonly JsonSerializerOptions Json =
        new(JsonSerializerDefaults.Web) { Converters = { new JsonStringEnumConverter() } };

    protected IntegrationTestBase(ApiFactory factory)
    {
        Factory = factory;
    }

    protected HttpClient CreateClient() => Factory.CreateClient();

    /// <summary>Регистрирует нового пользователя и возвращает данные аутентификации.</summary>
    protected async Task<AuthResponse> RegisterAsync(string? email = null, string displayName = "Test")
    {
        email ??= $"user_{Guid.NewGuid():N}@test.local";
        var res = await CreateClient().PostAsJsonAsync("/api/auth/register",
            new { email, password = "Passw0rd", displayName });
        res.EnsureSuccessStatusCode();
        return (await res.Content.ReadFromJsonAsync<AuthResponse>(Json))!;
    }

    /// <summary>Клиент с Bearer-токеном.</summary>
    protected HttpClient AuthedClient(string token)
    {
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    /// <summary>Логинится существующим админом (создаётся сидером).</summary>
    protected async Task<AuthResponse> LoginAdminAsync()
    {
        var res = await CreateClient().PostAsJsonAsync("/api/auth/login",
            new { email = "admin@resale.local", password = "Admin123$" });
        res.EnsureSuccessStatusCode();
        return (await res.Content.ReadFromJsonAsync<AuthResponse>(Json))!;
    }

    /// <summary>Создаёт объявление в первой листовой категории, возвращает его id.</summary>
    protected async Task<Guid> CreateListingAsync(HttpClient client, string title = "Товар", decimal price = 1000)
    {
        var categoryId = await FirstLeafCategoryIdAsync();
        var res = await client.PostAsJsonAsync("/api/listings", new
        {
            title,
            description = "Описание",
            price,
            condition = "Used",
            city = "Москва",
            categoryId,
        });
        res.EnsureSuccessStatusCode();
        var body = await res.Content.ReadFromJsonAsync<IdResponse>(Json);
        return body!.Id;
    }

    protected async Task<Guid> FirstLeafCategoryIdAsync()
    {
        var cats = await CreateClient().GetFromJsonAsync<List<CategoryNode>>("/api/categories", Json);
        return cats!.First().Children.First().Id;
    }
}

// --- Лёгкие модели ответов для десериализации ---

public record AuthResponse(string AccessToken, string RefreshToken, UserModel User);
public record UserModel(Guid Id, string Email, string DisplayName, string[] Roles);
public record IdResponse(Guid Id);
public record CategoryNode(Guid Id, string Name, string Slug, List<CategoryNode> Children);
public record ListingModel(Guid Id, string Title, decimal Price, string City, string Status, string SellerName, bool IsFavorite);
public record PagedListings(List<ListingModel> Items, int Page, int PageSize, int TotalCount, int TotalPages);
