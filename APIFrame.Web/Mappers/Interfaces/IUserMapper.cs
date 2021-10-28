using APIFrame.Core.DTOs;
using APIFrame.Core.Models;

namespace APIFrame.Web.Mappers.Interfaces
{
    public interface IUserMapper
    {
        BaseUserDTO MapToBaseUserDTO(BaseUser baseUser);
    }
}
