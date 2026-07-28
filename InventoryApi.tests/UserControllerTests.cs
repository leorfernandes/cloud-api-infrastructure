using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using InventoryApi.Dtos;
using Xunit;

namespace InventoryApi.Tests;

public class UsersControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public UsersControllerTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateUser_ReturnsCreatedWithCorrectFields()
    {
        var dto = new CreateUserDto { Username = "create_user_full", Email = "create_user_full@test.com" };

        var response = await _client.PostAsJsonAsync("/api/users", dto);
        var body = await response.Content.ReadFromJsonAsync<ReturnUserDto>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("create_user_full", body!.Username);
    }

    [Fact]
    public async Task GetUser_ExistingId_ReturnsOk()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/users",
            new CreateUserDto { Username = "get_ok_user_full", Email = "get_ok_user_full@test.com" });
        var created = await createResponse.Content.ReadFromJsonAsync<ReturnUserDto>();

        var getResponse = await _client.GetAsync($"/api/users/{created!.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    [Fact]
    public async Task GetUser_NonexistentId_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/users/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateUser_ExistingId_ReturnsNoContentAndPersists()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/users",
            new CreateUserDto { Username = "update_user_full", Email = "update_user_full@test.com" });
        var created = await createResponse.Content.ReadFromJsonAsync<ReturnUserDto>();

        var updateDto = new CreateUserDto { Username = "update_user_full_renamed", Email = "update_user_full@test.com" };
        var updateResponse = await _client.PutAsJsonAsync($"/api/users/{created!.Id}", updateDto);

        var getResponse = await _client.GetAsync($"/api/users/{created.Id}");
        var updated = await getResponse.Content.ReadFromJsonAsync<ReturnUserDto>();

        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);
        Assert.Equal("update_user_full_renamed", updated!.Username);
    }

    [Fact]
    public async Task DeleteUser_ExistingId_RemovesItAndReturns404OnFollowUp()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/users",
            new CreateUserDto { Username = "delete_user_full", Email = "delete_user_full@test.com" });
        var created = await createResponse.Content.ReadFromJsonAsync<ReturnUserDto>();

        var deleteResponse = await _client.DeleteAsync($"/api/users/{created!.Id}");
        var getResponse = await _client.GetAsync($"/api/users/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}