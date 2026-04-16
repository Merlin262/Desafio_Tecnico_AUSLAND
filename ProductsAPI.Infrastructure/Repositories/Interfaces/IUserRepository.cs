using ProductsAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductsAPI.Infrastructure.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<bool> ExistsAsync(string username);
        Task<User> CreateAsync(User user);
    }
}
