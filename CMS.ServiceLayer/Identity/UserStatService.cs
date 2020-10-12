using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CMS.DataLayer.Context;
using CMS.Entities.AuditableEntity;
using CMS.Entities.Common.Enums;
using CMS.Entities.Identity;
using CMS.ServiceLayer.Contracts.Identity;
using Microsoft.EntityFrameworkCore;

namespace CMS.ServiceLayer.Identity
{
    public class UserStatService : IUserStatService
    {
        private readonly IUnitOfWork _uow;
        private readonly IApplicationUserManager _userManager;
        private readonly DbSet<User> _users;

        public UserStatService
        (
            IApplicationUserManager userManager,
            IUnitOfWork uow
        )
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(_userManager));
            _uow = uow ?? throw new ArgumentNullException(nameof(_uow));
            _users = uow.Set<User>();
        }

        public Task<List<User>> GetOnlineUsersListAsync(int numbersToTake, int minutesToTake)
        {
            var now = DateTime.UtcNow;
            var minutes = now.AddMinutes(-minutesToTake);
            return _users.AsNoTracking()
                         .Where(user => user.LastVisitDateTime != null && user.LastVisitDateTime.Value <= now
                                        && user.LastVisitDateTime.Value >= minutes)
                         .OrderByDescending(user => user.LastVisitDateTime)
                         .Take(numbersToTake)
                         .ToListAsync();
        }

        public Task<List<User>> GetTodayBirthdayListAsync()
        {
            var now = DateTime.UtcNow;
            var day = now.Day;
            var month = now.Month;
            return _users.AsNoTracking()
                         .Where(user => user.BirthDate != null && user.Status == Status.Active
                                        && user.BirthDate.Value.Day == day
                                        && user.BirthDate.Value.Month == month)
                         .ToListAsync();
        }

        public async Task UpdateUserLastVisitDateTimeAsync(ClaimsPrincipal claimsPrincipal)
        {
            var user = await _userManager.GetUserAsync(claimsPrincipal);
            user.LastVisitDateTime = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
        }
    }
}