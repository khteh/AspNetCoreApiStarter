﻿using System;
using System.Reflection;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Web.Api.Core.DTO.UseCaseRequests;
using Xunit;
using System.Security.Cryptography.X509Certificates;
using Web.Api.Core.Accounts;
using Web.Api.Core.Auth;
using Grpc.Net.Client;

namespace Web.Api.IntegrationTests.Services
{
    public class AccountsServiceIntegrationTests : IClassFixture<CustomGrpcServerFactory<Startup>>
    {
        private ServiceProvider _serviceProvider;
        public AccountsServiceIntegrationTests(CustomGrpcServerFactory<Startup> factory)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault;
            _serviceProvider = factory.ServiceProvider;
        }

        [Fact]
        public async Task CanRegisterUserWithValidAccountDetails()
        {
            Accounts.AccountsClient client = _serviceProvider.GetRequiredService<Accounts.AccountsClient>();
            Assert.NotNull(client);
            // Act
            RegisterUserResponse response = await client.RegisterAsync(new RegisterUserRequest() {
                FirstName = "John",
                LastName = "Doe",
                Email = "jdoe@gmail.com",
                UserName = "johndoe",
                Password = "P@$$w0rd1"
            });//.ResponseAsync.DefaultTimeout();

            // Assert
            //Assert.AreEqual(deadline, options.Deadline);
            //Assert.AreEqual(cancellationToken, options.CancellationToken);
            Assert.NotNull(response);
            Assert.NotNull(response.Response);
            Assert.True(response.Response.Success);
            Assert.Null(response.Response.Errors);
            Assert.False(string.IsNullOrEmpty(response.Id));
        }
        [Fact]
        public async Task CanDeleteUserWithValidAccountDetails()
        {
            Accounts.AccountsClient client = _serviceProvider.GetRequiredService<Accounts.AccountsClient>();
            Assert.NotNull(client);
            DeleteUserResponse response = await client.DeleteAsync(new StringInputParameter() { Value = "deleteme"});
            Assert.NotNull(response);
            Assert.NotNull(response.Response);
            Assert.True(response.Response.Success);
            Assert.Null(response.Response.Errors);
            Assert.False(string.IsNullOrEmpty(response.Id));
        }

        [Fact]
        public async Task CantRegisterUserWithInvalidAccountDetails()
        {
            Accounts.AccountsClient client = _serviceProvider.GetRequiredService<Accounts.AccountsClient>();
            Assert.NotNull(client);
            // Act
            RegisterUserResponse response = await client.RegisterAsync(new RegisterUserRequest() {
                FirstName = "John",
                LastName = "Doe",
                Email = string.Empty,
                UserName = "johndoe",
                Password = "P@$$w0rd1"
            });//.ResponseAsync.DefaultTimeout();

            // Assert
            //Assert.AreEqual(deadline, options.Deadline);
            //Assert.AreEqual(cancellationToken, options.CancellationToken);
            Assert.NotNull(response);
            Assert.NotNull(response.Response);
            Assert.False(response.Response.Success);
            Assert.NotNull(response.Response.Errors);
            Assert.Single(response.Response.Errors);
            Assert.True(string.IsNullOrEmpty(response.Id));
            Assert.Equal(HttpStatusCode.BadRequest.ToString(), response.Response.Errors.First().Code);
            Assert.Equal("'Email' is not a valid email address.", response.Response.Errors.First().Description);
        }
        [Fact]
        public async Task CantDeleteUserWithInvalidAccountDetails()
        {
            Accounts.AccountsClient client = _serviceProvider.GetRequiredService<Accounts.AccountsClient>();
            Assert.NotNull(client);
            DeleteUserResponse response = await client.DeleteAsync(new StringInputParameter() { Value = "DeleteMeNot"});
            Assert.NotNull(response);
            Assert.NotNull(response.Response);
            Assert.False(response.Response.Success);
            Assert.NotNull(response.Response.Errors);
            Assert.Single(response.Response.Errors);
            Assert.True(string.IsNullOrEmpty(response.Id));
            Assert.Equal(HttpStatusCode.BadRequest.ToString(), response.Response.Errors.First().Code);
            Assert.Equal("Invalid user!", response.Response.Errors.First().Description);
        }
        [Fact]
        public async Task CanFindById()
        {
            Accounts.AccountsClient client = _serviceProvider.GetRequiredService<Accounts.AccountsClient>();
            Assert.NotNull(client);
            FindUserResponse response = await client.FindByIdAsync(new StringInputParameter() { Value = "41532945-599e-4910-9599-0e7402017fbe"});
            Assert.NotNull(response);
            Assert.NotNull(response.Response);
            Assert.True(response.Response.Success);
            Assert.Null(response.Response.Errors);
            Assert.False(string.IsNullOrEmpty(response.Id));
            Assert.Equal("41532945-599e-4910-9599-0e7402017fbe", response.Id);
        }
        [Fact]
        public async Task CanFindByUsername()
        {
            Accounts.AccountsClient client = _serviceProvider.GetRequiredService<Accounts.AccountsClient>();
            Assert.NotNull(client);
            FindUserResponse response = await client.FindByUserNameAsync(new StringInputParameter() { Value = "mickeymouse"}); // UserManager is NOT case sensitive!
            Assert.NotNull(response);
            Assert.NotNull(response.Response);
            Assert.True(response.Response.Success);
            Assert.Null(response.Response.Errors);
            Assert.False(string.IsNullOrEmpty(response.Id));
            Assert.Equal("41532945-599e-4910-9599-0e7402017fbe", response.Id);
        }
        [Fact]
        public async Task CanFindByEmail()
        {
            Accounts.AccountsClient client = _serviceProvider.GetRequiredService<Accounts.AccountsClient>();
            Assert.NotNull(client);
            //var httpResponse = await _client.GetAsync(WebUtility.UrlEncode("/accounts/email/mickey@mouse.com")); // UserManager is NOT case sensitive!
            FindUserResponse response = await client.FindByEmailAsync(new StringInputParameter() { Value = "mickey@mouse.com"}); // UserManager is NOT case sensitive!
            Assert.NotNull(response);
            Assert.NotNull(response.Response);
            Assert.True(response.Response.Success);
            Assert.Null(response.Response.Errors);
            Assert.False(string.IsNullOrEmpty(response.Id));
            Assert.Equal("41532945-599e-4910-9599-0e7402017fbe", response.Id);
        }
        [Fact]
        public async Task CanChangePasswordWithValidAccountDetails()
        {
            Accounts.AccountsClient accountsClient = _serviceProvider.GetRequiredService<Accounts.AccountsClient>();
            Auth.AuthClient authClient = _serviceProvider.GetRequiredService<Auth.AuthClient>();
            Assert.NotNull(accountsClient);
            Assert.NotNull(authClient);
            // Create User
            RegisterUserResponse response = await accountsClient.RegisterAsync(new RegisterUserRequest() {
                FirstName = "FirstName",
                LastName = "LastName",
                Email = "user@gmail.com",
                UserName = "user1",
                Password = "P@$$w0rd1"
            });//.ResponseAsync.DefaultTimeout();
            Assert.NotNull(response);
            Assert.NotNull(response.Response);
            Assert.True(response.Response.Success);
            Assert.Null(response.Response.Errors);
            Assert.False(string.IsNullOrEmpty(response.Id));

            // Login
            LoginResponse loginResponse = await authClient.LoginAsync(new LoginRequest() {
                UserName = "user1",
                Password = "P@$$w0rd1"
            });
            Assert.NotNull(loginResponse);
            Assert.NotNull(loginResponse.Response);
            Assert.True(loginResponse.Response.Success);
            Assert.Null(loginResponse.Response.Errors);
            Assert.NotNull(loginResponse.AccessToken);
            Assert.NotNull(loginResponse.AccessToken.Token);
            Assert.False(string.IsNullOrEmpty(loginResponse.RefreshToken));
            Assert.Equal(7200, loginResponse.AccessToken.ExpiresIn);

            // Change Password
            Web.Api.Core.Grpc.Response pwdResponse = await accountsClient.ChangePasswordAsync(new ChangePasswordRequest() {
                Id = response.Id,
                Password = "P@$$w0rd1",
                NewPassword = "P@$$w0rd2",
            });
            Assert.NotNull(pwdResponse);
            Assert.NotNull(pwdResponse);
            Assert.True(pwdResponse.Success);
            Assert.Null(pwdResponse.Errors);

            // Should fail login with previous password
            LoginResponse loginResponse1 = await authClient.LoginAsync(new LoginRequest() {
                UserName = "user1",
                Password = "P@$$w0rd1"
            });
            Assert.NotNull(loginResponse1);
            Assert.NotNull(loginResponse1.Response);
            Assert.False(loginResponse1.Response.Success);
            Assert.NotNull(loginResponse1.Response.Errors);
            Assert.Single(loginResponse1.Response.Errors);
            Assert.Null(loginResponse1.AccessToken);
            Assert.True(string.IsNullOrEmpty(loginResponse1.RefreshToken));
            Assert.Equal(HttpStatusCode.Unauthorized.ToString(), loginResponse1.Response.Errors.First().Code);
            Assert.Equal("Invalid username or password!", loginResponse1.Response.Errors.First().Description);

            // Login
            LoginResponse loginResponse2 = await authClient.LoginAsync(new LoginRequest() {
                UserName = "user1",
                Password = "P@$$w0rd2"
            });
            Assert.NotNull(loginResponse2);
            Assert.NotNull(loginResponse2.Response);
            Assert.True(loginResponse2.Response.Success);
            Assert.Null(loginResponse2.Response.Errors);
            Assert.NotNull(loginResponse2.AccessToken);
            Assert.NotNull(loginResponse2.AccessToken.Token);
            Assert.False(string.IsNullOrEmpty(loginResponse2.RefreshToken));
            Assert.Equal(7200, loginResponse2.AccessToken.ExpiresIn);
        }
        [Fact]
        public async Task CanResetPasswordWithValidAccountDetails()
        {
            Accounts.AccountsClient accountsClient = _serviceProvider.GetRequiredService<Accounts.AccountsClient>();
            Auth.AuthClient authClient = _serviceProvider.GetRequiredService<Auth.AuthClient>();
            Assert.NotNull(accountsClient);
            Assert.NotNull(authClient);
            // Create User
            RegisterUserResponse response = await accountsClient.RegisterAsync(new RegisterUserRequest() {
                FirstName = "FirstName",
                LastName = "LastName",
                Email = "user@gmail.com",
                UserName = "user1",
                Password = "P@$$w0rd1"
            });//.ResponseAsync.DefaultTimeout();
            Assert.NotNull(response);
            Assert.NotNull(response.Response);
            Assert.True(response.Response.Success);
            Assert.Null(response.Response.Errors);
            Assert.False(string.IsNullOrEmpty(response.Id));

            // Login
            LoginResponse loginResponse = await authClient.LoginAsync(new LoginRequest() {
                UserName = "user2",
                Password = "P@$$w0rd1"
            });
            Assert.NotNull(loginResponse);
            Assert.NotNull(loginResponse.Response);
            Assert.True(loginResponse.Response.Success);
            Assert.Null(loginResponse.Response.Errors);
            Assert.NotNull(loginResponse.AccessToken);
            Assert.NotNull(loginResponse.AccessToken.Token);
            Assert.False(string.IsNullOrEmpty(loginResponse.RefreshToken));
            Assert.Equal(7200, loginResponse.AccessToken.ExpiresIn);

            // Reset Password
            Web.Api.Core.Grpc.Response pwdResponse = await accountsClient.ResetPasswordAsync(new ResetPasswordRequest() {
                Id = response.Id,
                NewPassword = "P@$$w0rd1",
            });
            Assert.NotNull(pwdResponse);
            Assert.NotNull(pwdResponse);
            Assert.True(pwdResponse.Success);
            Assert.Null(pwdResponse.Errors);

            // Should fail login with previous password
            LoginResponse loginResponse1 = await authClient.LoginAsync(new LoginRequest() {
                UserName = "user2",
                Password = "P@$$w0rd1"
            });
            Assert.NotNull(loginResponse1);
            Assert.NotNull(loginResponse1.Response);
            Assert.False(loginResponse1.Response.Success);
            Assert.NotNull(loginResponse1.Response.Errors);
            Assert.Single(loginResponse1.Response.Errors);
            Assert.Null(loginResponse1.AccessToken);
            Assert.True(string.IsNullOrEmpty(loginResponse1.RefreshToken));
            Assert.Equal(HttpStatusCode.Unauthorized.ToString(), loginResponse1.Response.Errors.First().Code);
            Assert.Equal("Invalid username or password!", loginResponse1.Response.Errors.First().Description);

            // Login
            LoginResponse loginResponse2 = await authClient.LoginAsync(new LoginRequest() {
                UserName = "user2",
                Password = "P@$$w0rd2"
            });
            Assert.NotNull(loginResponse2);
            Assert.NotNull(loginResponse2.Response);
            Assert.True(loginResponse2.Response.Success);
            Assert.Null(loginResponse2.Response.Errors);
            Assert.NotNull(loginResponse2.AccessToken);
            Assert.NotNull(loginResponse2.AccessToken.Token);
            Assert.False(string.IsNullOrEmpty(loginResponse2.RefreshToken));
            Assert.Equal(7200, loginResponse2.AccessToken.ExpiresIn);
        }
    }
}