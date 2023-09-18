using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Core.Persistence.Abstract;
using Core.Persistence.DAOs;
using StackExchange.Redis;

namespace Core.Persistence.Concrete
{
    public class UserPersistence : IUserPersistence
    {
        private readonly IDatabase _database;
        private int _lastExistingId;
    
        public UserPersistence()
        {
            var redis = ConnectionMultiplexer.Connect("localhost");
            _database = redis.GetDatabase();

            _lastExistingId = (int) _database.StringGet("lastExistingId");
        }

        ~UserPersistence()
        {
            _database.StringSet("lastExistingId", _lastExistingId.ToString());
        }
    
        public async Task<UserDAO> GetUserAsync(string email)
        {
            string jsonResult = await _database.StringGetAsync(email);

            if (jsonResult is null)
                throw new KeyNotFoundException($"no such email exists {email}");

            var userDao = JsonSerializer.Deserialize<UserDAO>(jsonResult);

            if (userDao is null)
                throw new InvalidOperationException("error while getting user by email");

            return userDao;
        }

        public async Task AddUserAsync(UserDAO userDao)
        {
            if (userDao is null)
                throw new InvalidOperationException("userDao is null");

            Interlocked.Increment(ref _lastExistingId);
            userDao.Id = _lastExistingId.ToString();

            var userDaoJson = JsonSerializer.Serialize(userDao);

            await _database.StringSetAsync(userDao.Email, userDaoJson);
        }

        public async Task UpdateUserAsync(UserDAO userDao)
        {
             await _database.StringSetAsync(userDao.Email, JsonSerializer.Serialize(userDao));
        }
    }
}