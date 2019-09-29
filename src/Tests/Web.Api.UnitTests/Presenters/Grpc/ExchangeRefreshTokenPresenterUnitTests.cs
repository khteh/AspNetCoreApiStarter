﻿using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Web.Api.Models.Response;
using Web.Api.Core.DTO;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Presenters.Grpc;
using Xunit;

namespace Web.Api.UnitTests.Presenters
{
    public class GRPCExchangeRefreshTokenPresenterUnitTests
    {
        [Fact]
        public void Handle_GivenSuccessfulUseCaseResponse_SetsAccessToken()
        {
            // arrange
            const string token = "777888AAABBB";
            var presenter = new ExchangeRefreshTokenPresenter();

            // act
            presenter.Handle(new Core.DTO.UseCaseResponses.ExchangeRefreshTokenResponse(new AccessToken(token, 0), "", true));

            // assert
            Assert.NotNull(presenter.Response);
            Assert.NotNull(presenter.Response.Response);
            Assert.True(presenter.Response.Response.Success);
            Assert.False(presenter.Response.Response.Errors.Any());
            Assert.NotNull(presenter.Response.AccessToken);
            Assert.Equal(token, presenter.Response.AccessToken.Token);
        }

        [Fact]
        public void Handle_GivenSuccessfulUseCaseResponse_SetsRefreshToken()
        {
            // arrange
            const string token = "777888AAABBB";
            var presenter = new ExchangeRefreshTokenPresenter();

            // act
            presenter.Handle(new Core.DTO.UseCaseResponses.ExchangeRefreshTokenResponse(null, token, true));

            // assert
            Assert.NotNull(presenter.Response);
            Assert.True(presenter.Response.Response.Success);
            Assert.False(presenter.Response.Response.Errors.Any());
            Assert.False(string.IsNullOrEmpty(presenter.Response.RefreshToken));
            Assert.Equal(token, presenter.Response.RefreshToken);
        }

        [Fact]
        public void Handle_GivenFailedUseCaseResponse_SetsError()
        {
            // arrange
            var presenter = new ExchangeRefreshTokenPresenter();

            // act
            presenter.Handle(new Core.DTO.UseCaseResponses.ExchangeRefreshTokenResponse(new List<Error>() { new Error("InvalidToken", "Invalid Token!")}));

            // assert
            Assert.NotNull(presenter.Response);
            Assert.Null(presenter.Response.AccessToken);
            Assert.True(string.IsNullOrEmpty(presenter.Response.RefreshToken));
            Assert.NotNull(presenter.Response.Response.Errors);
            Assert.Single(presenter.Response.Response.Errors);
            Assert.Equal("InvalidToken", presenter.Response.Response.Errors.First().Code);
            Assert.Equal("Invalid Token!", presenter.Response.Response.Errors.First().Description);
        }
    }
}