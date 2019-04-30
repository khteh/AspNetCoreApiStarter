﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Web.Api.Core.Domain.Entities;
using Web.Api.Core.Dto;
using Web.Api.Core.Dto.GatewayResponses.Repositories;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.Specifications;
using Web.Api.Infrastructure.Identity;

namespace Web.Api.Infrastructure.Data.Repositories
{
    internal sealed class UserRepository : EfRepository<User>, IUserRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public UserRepository(UserManager<AppUser> userManager, IMapper mapper, AppDbContext appDbContext): base(appDbContext)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<CreateUserResponse> Create(string firstName, string lastName, string email, string userName, string password)
        {
            try
            {
                var appUser = new AppUser { Email = email, UserName = userName };
                var identityResult = await _userManager.CreateAsync(appUser, password);

                if (!identityResult.Succeeded) return new CreateUserResponse(appUser.Id, false, identityResult.Errors.Select(e => new Error(e.Code, e.Description)));

                var user = new User(firstName, lastName, appUser.Id.ToString(), appUser.UserName);
                _appDbContext.Users.Add(user);
                await _appDbContext.SaveChangesAsync();

                return new CreateUserResponse(appUser.Id, identityResult.Succeeded, identityResult.Succeeded ? null : identityResult.Errors.Select(e => new Error(e.Code, e.Description)));
            } catch (Exception e) {
                return new CreateUserResponse(null, false, new List<Error>() { new Error(null, e.Message) });
            }
        }

        public async Task<User> FindByName(string userName)
        {
            try
            {
                var appUser = await _userManager.FindByNameAsync(userName);
                return appUser == null ? null : _mapper.Map(appUser, await GetSingleBySpec(new UserSpecification(appUser.Id)), opt => opt.ConfigureMap(MemberList.None));
            } catch (Exception e)
            {
                return null;
            }
        }

        public async Task<bool> CheckPassword(User user, string password)
        {
            AppUser appUser = _mapper.Map<AppUser>(user);
            return await _userManager.CheckPasswordAsync(_mapper.Map<AppUser>(user), password);
        }
    }
}
