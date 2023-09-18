using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Persistence.Abstract;
using Core.Persistence.DAOs;
using Core.Services.Abstract;

namespace Core.Services.Concrete
{
    public class BonusService : IBonusService
    {
        private static readonly ConcurrentDictionary<string, List<DateTime>> PreviousConcurrentLogins = new ConcurrentDictionary<string, List<DateTime>>();

        private static readonly Dictionary<int, int> BonusesForDays = new Dictionary<int, int>
        {
            { 1, 10 },
            { 2, 30 },
            { 3, 70 },
            { 4, 120 },
            { 5, 180 },
            { 6, 250 },
            { 7, 500 },
        };

        private readonly IUserPersistence _userPersistence;

        public BonusService(IUserPersistence userPersistence)
        {
            _userPersistence = userPersistence;
        }

        public async Task GiveBonusAsync(UserDAO user)
        {
            int bonusDay;
            if (!PreviousConcurrentLogins.TryGetValue(user.Id, out var logins))
            {
                bonusDay = 1;
                PreviousConcurrentLogins.TryAdd(user.Id, new List<DateTime> { DateTime.Today });
            }
            else if (logins.Count == 7){
                bonusDay = 1;
                PreviousConcurrentLogins[user.Id] = new List<DateTime> { DateTime.Today };
            }
            else
            {
                var didGetTodayBonus = logins.Any(a => a == DateTime.Today);

                if (didGetTodayBonus)
                    return;

                bonusDay = logins.Count + 1;
                logins.Add(DateTime.Today);
            }

            user.Bonuses += BonusesForDays[bonusDay];
            await _userPersistence.UpdateUserAsync(user);
        }
    }
}