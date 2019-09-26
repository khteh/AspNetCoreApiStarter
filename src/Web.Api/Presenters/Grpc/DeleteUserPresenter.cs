﻿using System.Net;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Models.Response;
using Web.Api.Serialization;

namespace Web.Api.Presenters.Grpc
{
    public class DeleteUserPresenter : PresenterBase<UseCaseResponseMessage, Web.Api.Core.Accounts.DeleteUserResponse>
    {
    }
}