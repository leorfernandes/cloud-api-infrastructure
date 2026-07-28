using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using InventoryApi.Dtos;
using Xunit;

namespace InventoryApi.Tests;

public class ItemsControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ItemsControllerTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<int> CreateCategoryAsync(string name)
    {
        var response = await _client.PostAsJsonAsync("/api/categories", new CreateCategoryDto { Name = name });
        var created = await response.Content.ReadFromJsonAsync<ReturnCategoryDto>();
        return created!.Id;
    }

    private async Task<int> CreateUserAsync(string username, string email)
    {
        var response = await _client.PostAsJsonAsync("/api/users", new CreateUserDto { Username = username, Email = email });
        var created = await response.Content.ReadFromJsonAsync<ReturnUserDto>();
        return created!.Id;
    }

    [Fact]
    public async Task CreateItem_ReturnsCreatedWithCorrectFields()
    {
        var categoryId = await CreateCategoryAsync("TestCategory-Create");
        var userId = await CreateUserAsync("create_user", "create_user@test.com");

        var dto = new CreateItemDto
        {
            Name = "Test Laptop",
            Quantity = 3,
            CategoryId = categoryId,
            OwnerId = userId
        };

        var response = await _client.PostAsJsonAsync("/api/items", dto);
        var body = await response.Content.ReadFromJsonAsync<ReturnItemDto>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("Test Laptop", body!.Name);
        Assert.Equal(3, body.Quantity);
    }

    [Fact]
    public async Task GetItem_ExistingId_ReturnsOk()
    {
        var categoryId = await CreateCategoryAsync("TestCategory-GetOk");
        var userId = await CreateUserAsync("get_ok_user", "get_ok@test.com");
        var createResponse = await _client.PostAsJsonAsync("/api/items",
            new CreateItemDto { Name = "Item A", Quantity = 1, CategoryId = categoryId, OwnerId = userId });
        var created = await createResponse.Content.ReadFromJsonAsync<ReturnItemDto>();

        var getResponse = await _client.GetAsync($"/api/items/{created!.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    [Fact]
    public async Task GetItem_NonexistentId_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/items/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateItem_ExistingId_ReturnsNoContentAndPersists()
    {
        var categoryId = await CreateCategoryAsync("TestCategory-Update");
        var userId = await CreateUserAsync("update_user", "update_user@test.com");
        var createResponse = await _client.PostAsJsonAsync("/api/items",
            new CreateItemDto { Name = "Old Name", Quantity = 1, CategoryId = categoryId, OwnerId = userId });
        var created = await createResponse.Content.ReadFromJsonAsync<ReturnItemDto>();

        var updateDto = new CreateItemDto { Name = "New Name", Quantity = 5, CategoryId = categoryId, OwnerId = userId };
        var updateResponse = await _client.PutAsJsonAsync($"/api/items/{created!.Id}", updateDto);

        var getResponse = await _client.GetAsync($"/api/items/{created.Id}");
        var updated = await getResponse.Content.ReadFromJsonAsync<UpdateItemDto>();

        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);
        Assert.Equal("New Name", updated!.Name);
        Assert.Equal(5, updated.Quantity);
    }

    [Fact]
    public async Task DeleteItem_ExistingId_RemovesItAndReturns404OnFollowUp()
    {
        var categoryId = await CreateCategoryAsync("TestCategory-Delete");
        var userId = await CreateUserAsync("delete_user", "delete_user@test.com");
        var createResponse = await _client.PostAsJsonAsync("/api/items",
            new CreateItemDto { Name = "To Delete", Quantity = 1, CategoryId = categoryId, OwnerId = userId });
        var created = await createResponse.Content.ReadFromJsonAsync<ReturnItemDto>();

        var deleteResponse = await _client.DeleteAsync($"/api/items/{created!.Id}");
        var getResponse = await _client.GetAsync($"/api/items/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}