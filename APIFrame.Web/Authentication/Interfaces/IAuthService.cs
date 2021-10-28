using APIFrame.Core.Attributes;
using APIFrame.Core.DTOs;
using System.Threading.Tasks;

namespace APIFrame.Web.Authentication.Interfaces
{
    [ExceptDynamicResolve]
    public interface IAuthService
    {
        Task<T> LogInAsync<T>(LoginRequestDTO loginDTO, string clientIp) where T : BaseUserDTO;
        Task<T> RegisterAsync<T>(RegisterRequestDTO registerDTO, string clientIp) where T : BaseUserDTO;
    }
}
