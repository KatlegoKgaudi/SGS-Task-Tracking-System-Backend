﻿using SGS.TaskTracker.Models;

namespace SGS.TaskTracker.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllAsync();
        Task<bool> UserExistsAsync(string username, string email);
        Task AddAsync(User user);
        void Update(User user);
        void Delete(User user);
        Task<int> SaveChangesAsync();
    }
}
