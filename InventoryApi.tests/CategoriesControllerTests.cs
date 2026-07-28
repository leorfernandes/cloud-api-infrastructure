using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using InventoryApi.Dtos;
using Xunit;

namespace InventoryApi.Tests;

public class CategoriesControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CategoriesControllerTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateCategory_ReturnsCreatedWithCorrectFields()
    {
        var dto = new CreateCategoryDto { Name = "Category-Create" };

        var response = await _client.PostAsJsonAsync("/api/categories", dto);
        var body = await response.Content.ReadFromJsonAsync<ReturnCategoryDto>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("Category-Create", body!.Name);
    }

    [Fact]
    public async Task GetCategory_ExistingId_ReturnsOk()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/categories",
            new CreateCategoryDto { Name = "Category-GetOk" });
        var created = await createResponse.Content.ReadFromJsonAsync<ReturnCategoryDto>();

        var getResponse = await _client.GetAsync($"/api/categories/{created!.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    [Fact]
    public async Task UpdateCategory_ExistingId_ReturnsNoContentAndPersists()
    {
        var dto = new CreateCategoryDto { Name = "Category-Update" };
        var createResponse = await _client.PostAsJsonAsync("/api/categories", dto);
        var created = await createResponse.Content.ReadFromJsonAsync<ReturnCategoryDto>();

        var updateDto = new CreateCategoryDto { Name = "New Name" };
        var updateResponse = await _client.PutAsJsonAsync($"/api/categories/{created!.Id}", updateDto);

        var getResponse = await _client.GetAsync($"/api/categories/{created.Id}");
        var updated = await getResponse.Content.ReadFromJsonAsync<ReturnCategoryDto>();

        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);
        Assert.Equal("New Name", updated!.Name);
    }

    [Fact]
    public async Task GetCategory_NonexistentId_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/categories/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCategory_ExistingId_RemovesItAndReturns404OnFollowUp()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/categories",
            new CreateCategoryDto { Name = "Category-Delete" });
        var created = await createResponse.Content.ReadFromJsonAsync<ReturnCategoryDto>();

        var deleteResponse = await _client.DeleteAsync($"/api/categories/{created!.Id}");
        var getResponse = await _client.GetAsync($"/api/categories/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}