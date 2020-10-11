﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMS.Common.IdentityToolkit;
using CMS.DataLayer.Context;
using CMS.Entities.Identity;
using CMS.ServiceLayer.Contracts.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CMS.ServiceLayer.Identity
{
    /// <summary>
    /// More info: http://www.dotnettips.info/post/2578
    /// </summary>
    public class ApplicationRoleManager : RoleManager<Role>, IApplicationRoleManager
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUnitOfWork _uow;
        private readonly IdentityErrorDescriber _errors;
        private readonly ILookupNormalizer _keyNormalizer;
        private readonly ILogger<ApplicationRoleManager> _logger;
        private readonly IEnumerable<IRoleValidator<Role>> _roleValidators;
        private readonly IApplicationRoleStore _store;
        private readonly DbSet<User> _users;

        public ApplicationRoleManager
        (
            IApplicationRoleStore store,
            IEnumerable<IRoleValidator<Role>> roleValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            ILogger<ApplicationRoleManager> logger,
            IHttpContextAccessor contextAccessor,
            IUnitOfWork uow
        ) : base((RoleStore<Role, ApplicationDbContext, int, UserRole, RoleClaim>) store, roleValidators, keyNormalizer, errors, logger)
        {
            _store = store ?? throw new ArgumentNullException(nameof(_store));
            _roleValidators = roleValidators ?? throw new ArgumentNullException(nameof(_roleValidators));
            _keyNormalizer = keyNormalizer ?? throw new ArgumentNullException(nameof(_keyNormalizer));
            _errors = errors ?? throw new ArgumentNullException(nameof(_errors));
            _logger = logger ?? throw new ArgumentNullException(nameof(_logger));
            _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(_contextAccessor));
            _uow = uow ?? throw new ArgumentNullException(nameof(_uow));
            _users = _uow.Set<User>();
        }

        #region BaseClass

        #endregion

        #region CustomMethods

        public IList<Role> FindCurrentUserRoles()
        {
            var userId = getCurrentUserId();
            return FindUserRoles(userId);
        }

        public IList<Role> FindUserRoles(int userId)
        {
            var userRolesQuery = from role in Roles
                                 from user in role.Users
                                 where user.UserId == userId
                                 select role;

            return userRolesQuery.OrderBy(x => x.Name).ToList();
        }

        public Task<List<Role>> GetAllCustomRolesAsync()
        {
            return Roles.ToListAsync();
        }

        public IList<User> GetApplicationUsersInRole(string roleName)
        {
            var roleUserIdsQuery = from role in Roles
                                   where role.Name == roleName
                                   from user in role.Users
                                   select user.UserId;
            return _users.Where(applicationUser => roleUserIdsQuery.Contains(applicationUser.Id))
                         .ToList();
        }

        public IList<Role> GetRolesForCurrentUser()
        {
            var userId = getCurrentUserId();
            return GetRolesForUser(userId);
        }

        public IList<Role> GetRolesForUser(int userId)
        {
            var roles = FindUserRoles(userId);
            if (roles == null || !roles.Any())
            {
                return new List<Role>();
            }

            return roles.ToList();
        }

        public IList<UserRole> GetUserRolesInRole(string roleName)
        {
            return Roles.Where(role => role.Name == roleName)
                             .SelectMany(role => role.Users)
                             .ToList();
        }

        public bool IsCurrentUserInRole(string roleName)
        {
            var userId = getCurrentUserId();
            return IsUserInRole(userId, roleName);
        }

        public bool IsUserInRole(int userId, string roleName)
        {
            var userRolesQuery = from role in Roles
                                 where role.Name == roleName
                                 from user in role.Users
                                 where user.UserId == userId
                                 select role;
            var userRole = userRolesQuery.FirstOrDefault();
            return userRole != null;
        }

        public Task<Role> FindRoleIncludeRoleClaimsAsync(int roleId)
        {
            return Roles.Include(x => x.Claims).FirstOrDefaultAsync(x => x.Id == roleId);
        }

        public async Task<IdentityResult> AddOrUpdateRoleClaimsAsync(
            int roleId,
            string roleClaimType,
            IList<string> selectedRoleClaimValues)
        {
            var role = await FindRoleIncludeRoleClaimsAsync(roleId);
            if (role == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "RoleNotFound",
                    Description = "نقش مورد نظر یافت نشد."
                });
            }

            var currentRoleClaimValues = role.Claims.Where(roleClaim => roleClaim.ClaimType == roleClaimType)
                                                    .Select(roleClaim => roleClaim.ClaimValue)
                                                    .ToList();

            if (selectedRoleClaimValues == null)
            {
                selectedRoleClaimValues = new List<string>();
            }
            var newClaimValuesToAdd = selectedRoleClaimValues.Except(currentRoleClaimValues).ToList();
            foreach (var claimValue in newClaimValuesToAdd)
            {
                role.Claims.Add(new RoleClaim
                {
                    RoleId = role.Id,
                    ClaimType = roleClaimType,
                    ClaimValue = claimValue
                });
            }

            var removedClaimValues = currentRoleClaimValues.Except(selectedRoleClaimValues).ToList();
            foreach (var claimValue in removedClaimValues)
            {
                var roleClaim = role.Claims.SingleOrDefault(rc => rc.ClaimValue == claimValue &&
                                                                  rc.ClaimType == roleClaimType);
                if (roleClaim != null)
                {
                    role.Claims.Remove(roleClaim);
                }
            }

            return await UpdateAsync(role);
        }

        private int getCurrentUserId() => _contextAccessor.HttpContext.User.Identity.GetUserId<int>();

        #endregion
    }
}
