﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Web.Api.Core.DTO;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Models.Response;
using Web.Api.Presenters;
using Xunit;

namespace Web.Api.UnitTests.Presenters
{
    public class DeleteUserPresenterUnitTests
    {
        [Fact]
        public void Handle_GivenSuccessfulUseCaseResponse_SetsOKHttpStatusCode()
        {
            // arrange
            var presenter = new DeleteUserPresenter();

            // act
            presenter.Handle(new UseCaseResponseMessage("", true));

            // assert
            Assert.Equal((int)HttpStatusCode.OK, presenter.ContentResult.StatusCode);
        }

        [Fact]
        public void Handle_GivenSuccessfulUseCaseResponse_SetsId()
        {
            // arrange
            var presenter = new DeleteUserPresenter();

            // act
            presenter.Handle(new UseCaseResponseMessage("1234", true));

            // assert
            DeleteUserResponse response = Serialization.JsonSerializer.DeSerializeObject<DeleteUserResponse>(presenter.ContentResult.Content);
            Assert.Equal((int)HttpStatusCode.OK, presenter.ContentResult.StatusCode);
            Assert.NotNull(response);
            Assert.True(response.Success);
            Assert.Null(response.Errors);
        }

        [Fact]
        public void Handle_GivenFailedUseCaseResponse_SetsErrors()
        {
            // arrange
            var presenter = new DeleteUserPresenter();

            // act
            presenter.Handle(new UseCaseResponseMessage(new List<Error>() { new Error(HttpStatusCode.BadRequest.ToString(), "Invalid user!") }));

            // assert
            DeleteUserResponse response = Serialization.JsonSerializer.DeSerializeObject<DeleteUserResponse>(presenter.ContentResult.Content);
            Assert.Equal((int)HttpStatusCode.BadRequest, presenter.ContentResult.StatusCode);
            Assert.NotNull(response);
            Assert.NotNull(response.Errors);
            Assert.NotEmpty(response.Errors);
            Assert.False(string.IsNullOrEmpty(response.Errors.First().Code));
            Assert.False(string.IsNullOrEmpty(response.Errors.First().Description));
            Assert.Equal(HttpStatusCode.BadRequest.ToString(), response.Errors.First().Code);
            Assert.Equal("Invalid user!", response.Errors.First().Description);
        }
    }
}