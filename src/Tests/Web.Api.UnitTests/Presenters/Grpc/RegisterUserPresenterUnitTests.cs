﻿using System.Linq;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using Web.Api.Core.DTO;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Presenters.Grpc;
using Xunit;
using Web.Api.Core.Interfaces;

namespace Web.Api.UnitTests.Presenters
{
    public class GRPCRegisterUserPresenterUnitTests
    {
        [Fact]
        public void Handle_GivenSuccessfulUseCaseResponse_SetsOKHttpStatusCode()
        {
            // arrange
            var presenter = new RegisterUserPresenter();

            // act
            presenter.Handle(new UseCaseResponseMessage("", true));

            // assert
            Assert.NotNull(presenter.Response);
            Assert.NotNull(presenter.Response.Response);
            Assert.True(presenter.Response.Response.Success);
            Assert.False(presenter.Response.Response.Errors.Any());
        }

        [Fact]
        public void Handle_GivenSuccessfulUseCaseResponse_SetsId()
        {
            // arrange
            var presenter = new RegisterUserPresenter();

            // act
            presenter.Handle(new UseCaseResponseMessage("1234", true));

            // assert
            Assert.NotNull(presenter.Response);
            Assert.NotNull(presenter.Response.Response);
            Assert.True(presenter.Response.Response.Success);
            Assert.False(presenter.Response.Response.Errors.Any());
            Assert.Equal("1234", presenter.Response.Id);
        }

        [Fact]
        public void Handle_GivenFailedUseCaseResponse_SetsErrors()
        {
            // arrange
            var presenter = new RegisterUserPresenter();

            // act
            presenter.Handle(new UseCaseResponseMessage(new List<Error>() { new Error(HttpStatusCode.BadRequest.ToString(), "missing first name") }));

            // assert
            Assert.NotNull(presenter.Response);
            Assert.NotNull(presenter.Response.Response.Errors);
            Assert.NotEmpty(presenter.Response.Response.Errors);
            Assert.False(string.IsNullOrEmpty(presenter.Response.Response.Errors.First().Code));
            Assert.False(string.IsNullOrEmpty(presenter.Response.Response.Errors.First().Description));
            Assert.Equal(HttpStatusCode.BadRequest.ToString(), presenter.Response.Response.Errors.First().Code);
            Assert.Equal("missing first name", presenter.Response.Response.Errors.First().Description);
        }
    }
}