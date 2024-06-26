using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NiallMaloney.EventSourcing;
using NiallMaloney.PendingCreation.Service.Users.Controllers.Models;
using NiallMaloney.Shared.TestUtils;

namespace NiallMaloney.PendingCreation.IntegrationTests;

public class UsersTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public UsersTests(WebApplicationFactory<Program> app)
    {
        _client = app.CreateClient();
        app.Services.GetRequiredService<EventStoreClient>();
    }

    [Fact]
    public async Task Given_A_Unique_User_Request_Then_User_Is_Active()
    {
        //Arrange
        var emailAddress = $"{Guid.NewGuid().ToString()[..6]}@example.com";
        var forename = "Bruce";
        var surname = "Wayne";

        //Act
        var userRef = await CreateUser(emailAddress, forename, surname);
        var userId = userRef.UserId;
        var user = await GetOrWaitForExpectedUser(userId, "Active");

        //Assert
        using var scope = new AssertionScope();

        user.Should().NotBeNull();
        user.EmailAddress.Should().Be(emailAddress);
        user.Forename.Should().Be(forename);
        user.Surname.Should().Be(surname);
        user.Status.Should().Be("Active");
    }

    [Fact]
    public async Task Given_A_User_With_Same_Email_Exists_When_Adding_User_Then_User_Is_Rejected()
    {
        //Arrange
        var emailAddress = $"{Guid.NewGuid().ToString()[..6]}@example.com";
        var forename = "Bruce";
        var surname = "Wayne";

        //Act
        var firstUserRef = await CreateUser(emailAddress, forename, surname);
        var firstUserId = firstUserRef.UserId;

        var secondUserRef = await CreateUser(emailAddress, forename, surname);
        var secondUserId = secondUserRef.UserId;

        var firstUser = await GetOrWaitForExpectedUser(firstUserId, "Active");
        var secondUser = await GetOrWaitForExpectedUser(secondUserId, "Rejected");

        //Assert
        using var scope = new AssertionScope();

        firstUser.Status.Should().Be("Active");
        secondUser.Status.Should().Be("Rejected");
    }

    private async Task<User> GetOrWaitForExpectedUser(
        string userId,
        string? expectedStatus = null
    )
    {
        var user = await TestHelpers.RetryUntil(
            async () => await GetUser(userId),
            u => expectedStatus is null || u?.Status == expectedStatus
        );
        user.Should().NotBeNull();
        return user!;
    }

    private async Task<User?> GetUser(string userId)
    {
        var getResponseMessage = await _client.GetAsync($"/users/{userId}");
        if (getResponseMessage.IsSuccessStatusCode)
        {
            return await getResponseMessage.Content.ReadFromJsonAsync<User>();
        }

        if (getResponseMessage.StatusCode is HttpStatusCode.NotFound)
        {
            return null;
        }

        throw new HttpRequestException();
    }

    private async Task<UserReference> CreateUser(
        string emailAddress,
        string forename,
        string surname,
        bool assertSuccess = true
    )
    {
        var postResponseMessage = await _client.PostAsJsonAsync(
            "/users",
            new
            {
                EmailAddress = emailAddress,
                Forename = forename,
                Surname = surname,
            }
        );
        if (assertSuccess)
        {
            postResponseMessage.IsSuccessStatusCode.Should().BeTrue();
        }

        var userRef = await postResponseMessage.Content.ReadFromJsonAsync<UserReference>();
        userRef.Should().NotBeNull();
        return userRef!;
    }
}