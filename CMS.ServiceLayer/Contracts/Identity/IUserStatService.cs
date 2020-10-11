using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using CMS.Entities.Identity;

namespace CMS.ServiceLayer.Contracts.Identity
{
    public interface IUserStatService
    {
        Task<List<User>> GetOnlineUsersListAsync(int numbersToTake, int minutesToTake);

        Task<List<User>> GetTodayBirthdayListAsync();

        Task UpdateUserLastVisitDateTimeAsync(ClaimsPrincipal claimsPrincipal);
    }
}