using APIFrame.Core.DTOs;
using APIFrame.Core.Exceptions;
using APIFrame.Core.Mappers.Interfaces;
using APIFrame.Core.Models;
using APIFrame.Core.Utils;
using APIFrame.DataAccess.Repositories.Interfaces;
using APIFrame.Web.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace APIFrame.Web.Services
{
    public class AuthService : IAuthService
    {
        private readonly IPasswordService _passwordService;
        private readonly StringGeneratorUtil _stringGeneratorUtil;

        private readonly IBaseUserRepo<BaseUser> _userRepo;

        private readonly IUserMapper _userMapper;

        public AuthService(
            IPasswordService passwordService,
            StringGeneratorUtil stringGeneratorService,
            IBaseUserRepo<BaseUser> userRepo,
            IUserMapper baseUserMapper)
        {
            _passwordService = passwordService;
            _stringGeneratorUtil = stringGeneratorService;
            _userRepo = userRepo;
            _userMapper = baseUserMapper;
        }

        public virtual async Task<T> LogInAsync<T>(
            LoginRequestDTO loginDTO,
            string clientIp) where T : BaseUserDTO
        {
            var user = await _userRepo.GetByEmailAsync(loginDTO.UserNameOrEmail);
            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            var passwordHash = _passwordService.CreateHash(loginDTO.Password, user.PasswordSalt);
            if (!user.PasswordHash.Equals(passwordHash, StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException();
            }

            if (user.IsBanned)
            {
                throw new BannedUserException(user.Email);
            }

            user.AuthDate = DateTime.Now;
            user.AuthIP = clientIp;

            await _userRepo.UpdateAsync(user);

            return _userMapper.MapToBaseUserDTO(user) as T;
        }

        public virtual async Task<T> RegisterAsync<T>(
            RegisterRequestDTO registerDTO,
            string clientIp) where T : BaseUserDTO
        {
            var existingUser = await _userRepo.GetByEmailAsync(registerDTO.Email);
            if (existingUser != null)
            {
                throw new UserTakenException(existingUser.Email);
            }

            var passwordSalt = _stringGeneratorUtil.GetRandomString(64);
            var passwordHash = _passwordService.CreateHash(registerDTO.Password, passwordSalt);

            var registeredUser = await _userRepo.AddAsync(new BaseUser()
            {
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Email = registerDTO.Email,
                AuthIP = clientIp
            });

            return _userMapper.MapToBaseUserDTO(registeredUser) as T;
        }
    }
}
