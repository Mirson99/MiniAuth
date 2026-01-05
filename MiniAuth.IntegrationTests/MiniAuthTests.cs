using System.Net;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MiniAuth.API.Controllers;
using MiniAuth.Application.Auth.Commands.Login;
using MiniAuth.Application.Auth.Commands.LoginWithRefreshToken;
using MiniAuth.Application.Auth.Commands.Register;
using MiniAuth.Application.Responses;
using MiniAuth.Domain.Entities;
using MiniAuth.Infrastructure.Data;
using MiniAuth.IntegrationTests.Fixtures;
using MiniAuth.IntegrationTests.Helpers;

namespace MiniAuth.IntegrationTests;

public class MiniAuthTests : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly IntegrationTestWebAppFactory _factory;

    public MiniAuthTests(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public void CleanDatabase(AuthDbContext context)
    {
        context.RolePermissions.RemoveRange(context.RolePermissions);
        context.RefreshTokens.RemoveRange(context.RefreshTokens);
        context.Users.RemoveRange(context.Users);
        context.Roles.RemoveRange(context.Roles);
        context.Permissions.RemoveRange(context.Permissions);
    
        context.SaveChanges();
    }
    public void SeedTestData(AuthDbContext context)
    {
        CleanDatabase(context);
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId, Email = "user@test.com",
            PasswordHash = "MTVTL2JQnh3r9JqndmFuqDLde7FGFt8+yCNjgn4h547kcVRmGL2MX6bMMbA4L3bw", CreatedAt = DateTime.Now,
            LastLoginAt = null
        };
        var role = new Role { Id = 1, Name = "Registered" };
        user.Roles.Add(role);
        context.Users.Add(user);
        context.Roles.Add(role);
        context.Permissions.Add(new Permission {Id=1, Name = "ReadMember"});
        context.Permissions.Add(new Permission {Id=2, Name = "UpdateMember"});
        context.Permissions.Add(new Permission {Id=3, Name = "WriteMember"});
        context.RolePermissions.Add(new RolePermission {RoleId = 1, PermissionId = 1});
        context.RolePermissions.Add(new RolePermission {RoleId = 1, PermissionId = 2});
        context.SaveChanges();
    }


    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
    
    [Fact]
    public async Task Register_WithValidData_ValidationSuccess()
    {
        // Arrange
        var request = new RegisterCommand("lukmir2@gmail.com", "Password123@!");
    
        // Act
        var response = await _client.PostAsJsonAsync("/auth/register", request);
    
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<RegisterResponse>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        result.Should().NotBe(null);
        result.Id.Should().NotBeEmpty();
    }
    
    [Fact]
    public async Task Register_WithTooShortPassword_ValidationFails()
    {
        // Arrange
        var request = new RegisterCommand("test2@test.com", "Pass");
    
        // Act
        var response = await _client.PostAsJsonAsync("/auth/register", request);
    
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Password must be at least 8 characters");
    }
    
    [Fact]
    public async Task Login_WithoutEmail_ValidationFails()
    {
        // Arrange
        var request = new LoginCommand("", "Pass");
    
        // Act
        var response = await _client.PostAsJsonAsync("/auth/login", request);
    
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Email is required");
    }
    
    [Fact]
    public async Task Login_WithValidData_ValidationSuccess()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        SeedTestData(context);
        var request = new LoginCommand("user@test.com", "password");
    
        // Act
        var response = await _client.PostAsJsonAsync("/auth/login", request);
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<LoginResponse>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        result.Should().NotBe(null);
        result.RefreshToken.Should().NotBeEmpty();
        result.Token.Should().NotBeEmpty();
    }
    
    [Fact]
    public async Task LoginGetRefreshToken_WithValidData_ValidationSuccess()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        SeedTestData(context);
        var request = new LoginCommand("user@test.com", "password");
        var response = await _client.PostAsJsonAsync("/auth/login", request);
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<LoginResponse>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        result.Should().NotBe(null);
        result.RefreshToken.Should().NotBeEmpty();
        result.Token.Should().NotBeEmpty();
        var requestRefresh = new LoginWithRefreshTokenCommand(result.RefreshToken);
    
        // Act
        var responseRefresh = await _client.PostAsJsonAsync("/auth/refresh-token", requestRefresh);
        
        // Assert
        var contentRefresh = await responseRefresh.Content.ReadAsStringAsync();
        var resultRefresh = JsonSerializer.Deserialize<LoginResponse>(contentRefresh,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        responseRefresh.StatusCode.Should().Be(HttpStatusCode.OK);
        resultRefresh.Should().NotBe(null);
        resultRefresh.RefreshToken.Should().NotBeEmpty();
        resultRefresh.Token.Should().NotBeEmpty();
    }
    
    [Fact]
    public async Task LoginAndGetWriteExample_WithoutRole_Forbidden()
    {
        //Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        SeedTestData(context);
        var request = new LoginCommand("user@test.com", "password");
        var response = await _client.PostAsJsonAsync("/auth/login", request);
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<LoginResponse>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        result.Should().NotBe(null);
        result.RefreshToken.Should().NotBeEmpty();
        result.Token.Should().NotBeEmpty();
    
        // Act
        var getRequest = new HttpRequestMessage(HttpMethod.Get, "/example/write");
        getRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
            "Bearer", 
            result.Token);
        var responseRefresh = await _client.SendAsync(getRequest);

        
        // Assert
        responseRefresh.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    [Fact]
    public async Task LoginAndGetExample_WithRole_Success()
    {
        //Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        SeedTestData(context);
        var request = new LoginCommand("user@test.com", "password");
        var response = await _client.PostAsJsonAsync("/auth/login", request);
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<LoginResponse>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        result.Should().NotBe(null);
        result.RefreshToken.Should().NotBeEmpty();
        result.Token.Should().NotBeEmpty();
    
        // Act
        var getRequest = new HttpRequestMessage(HttpMethod.Get, "/example");
        getRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
            "Bearer", 
            result.Token);
        var responseRefresh = await _client.SendAsync(getRequest);

        
        // Assert
        responseRefresh.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}