using APIFrame.Core.Constants;
using APIFrame.Core.DTOs;
using APIFrame.Core.Exceptions;
using APIFrame.Core.Mappers.Interfaces;
using APIFrame.Core.Models;
using APIFrame.DataAccess.Repositories.Interfaces;
using APIFrame.Utils.String;
using APIFrame.Web.Authentication.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace APIFrame.Web.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly IJwtGeneratorService _jwtGeneratorService;
        private readonly IAntiforgeryService _antiforgeryService;
        private readonly ISecureHashGeneratorService _hashGeneratorService;
        private readonly StringGenerator _stringGenerator;
        private readonly IContextInfo _contextInfo;
        private readonly IBaseUserRepo<BaseUser> _userRepo;

        private readonly IUserMapper _userMapper;

        public AuthService(
            IContextInfo contextInfo,
            IAntiforgeryService antiforgeryService,
            IJwtGeneratorService jwtGeneratorService,
            ISecureHashGeneratorService hashGeneratorService,
            StringGenerator stringGenerator,
            IBaseUserRepo<BaseUser> userRepo,
            IUserMapper baseUserMapper)
        {
            _contextInfo = contextInfo;
            _antiforgeryService = antiforgeryService;
            _jwtGeneratorService = jwtGeneratorService;
            _hashGeneratorService = hashGeneratorService;
            _stringGenerator = stringGenerator;
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

            var passwordHash = _hashGeneratorService.CreateHash(loginDTO.Password, user.PasswordSalt);
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

            UpdateContextInfo(user.Email);

            return _userMapper.MapToBaseUserDTO(user) as T;
        }

        private void UpdateContextInfo(string userId)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimConstants.UserId, userId),
                new Claim(ClaimConstants.ClientIp, _contextInfo.ClientIp)
            };

            _contextInfo.AuthToken = _jwtGeneratorService.GenerateJwtToken(claims);
            _contextInfo.AntiforgeryToken = _antiforgeryService.GenerateAntiforgeryToken();
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

            var passwordSalt = _stringGenerator.GetRandomString(64);
            var passwordHash = _hashGeneratorService.CreateHash(registerDTO.Password, passwordSalt);

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
