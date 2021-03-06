﻿using System.Net;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Serialization;

namespace Web.Api.Presenters
{
    public class FindUserPresenter : PresenterBase<FindUserResponse, Models.Response.FindUserResponse>
    {
        public override void Handle(FindUserResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
            ContentResult.Content = JsonSerializer.SerializeObject(new Models.Response.FindUserResponse(response.Id, response.Message, response.User, response.Success, response.Errors));
        }
    }
}