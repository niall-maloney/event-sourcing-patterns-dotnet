using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NiallMaloney.EventSourcing;
using NiallMaloney.FatEvents.Service;
using NiallMaloney.FatEvents.Service.Notifications.Domain;
using NiallMaloney.FatEvents.Service.Users.Controllers.Models;
using NiallMaloney.Shared.TestUtils;
using Shouldly;
using Notification = NiallMaloney.FatEvents.Service.Notifications.Controllers.Models.Notification;

namespace NiallMaloney.FatEvents.IntegrationTests;

public class NotificationsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public NotificationsTests(WebApplicationFactory<Program> app)
    {
        _client = app.CreateClient();
        app.Services.GetRequiredService<EventStoreClient>();
    }

    [Fact]
    public async Task Given_A_User_Request_When_Accepted_Then_Notification_Is_Sent()
    {
        //Arrange
        var emailAddress = $"{Guid.NewGuid().ToString()[..6]}@example.com";
        var forename = "Bruce";
        var surname = "Wayne";

        //Act
        var userRef = await CreateUser(emailAddress, forename, surname);
        var userId = userRef.UserId;
        var user = await GetOrWaitForExpectedUser(userId, "Active");

        //lean on deterministic ids
        var notificationId = Ids.NewNotificationId(userId, NotificationTypes.UserAccepted);
        var notification = await GetOrWaitForExpectedNotification(notificationId, "Sent");

        //Assert
        user.ShouldSatisfyAllConditions(
            () => user.ShouldNotBeNull(),
            () => user.EmailAddress.ShouldBe(emailAddress),
            () => user.Forename.ShouldBe(forename),
            () => user.Surname.ShouldBe(surname),
            () => user.Status.ShouldBe("Active"));

        notification.ShouldSatisfyAllConditions(
            () => notification.ShouldNotBeNull(),
            () => notification.Type.ShouldBe(NotificationTypes.UserAccepted),
            () => notification.Status.ShouldBe("Sent"));
    }

    private async Task<Notification> GetOrWaitForExpectedNotification(
        string notificationId,
        string? expectedStatus = null
    )
    {
        var notification = await TestHelpers.RetryUntil(
            async () => await GetNotification(notificationId),
            u => expectedStatus is null || u?.Status == expectedStatus
        );
        notification.ShouldNotBeNull();
        return notification;
    }

    private async Task<Notification?> GetNotification(string notificationId)
    {
        var getResponseMessage = await _client.GetAsync($"/notifications/{notificationId}");
        if (getResponseMessage.IsSuccessStatusCode)
        {
            return await getResponseMessage.Content.ReadFromJsonAsync<Notification>();
        }

        if (getResponseMessage.StatusCode is HttpStatusCode.NotFound)
        {
            return null;
        }

        throw new HttpRequestException();
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
        user.ShouldNotBeNull();
        return user;
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
            postResponseMessage.IsSuccessStatusCode.ShouldBeTrue();
        }

        var userRef = await postResponseMessage.Content.ReadFromJsonAsync<UserReference>();
        userRef.ShouldNotBeNull();
        return userRef;
    }
}
