using System;
using System.Threading.Tasks;

namespace AlunosApi.Services
{
    public interface IUserService
    {

        Task<bool> Authenticate(String email, String password);

        Task<bool> RegisterUser(String email, String password);

        Task Logout();
    }
}

