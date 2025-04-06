using PM.Identity.Application.Implements.Flows;
using PM.Identity.Application.Interfaces.Services;
using PM.Identity.Domain.Entities;
using PM.Shared.Dtos;
using PM.Shared.Dtos.Auths;

namespace PM.Identity.Application.Interfaces.Flows
{
    public class AuthFlow : IAuthFlow
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenService _refreshTokenService;
        public AuthFlow(IAuthService authService, IUserService userService, ITokenService tokenService, IRefreshTokenService refreshTokenService)
        {
            _authService = authService;
            _userService = userService;
            _tokenService = tokenService;
            _refreshTokenService = refreshTokenService;
        }
        public async Task<ServiceResult<string>> Login(LoginModel loginModel)
        {
            var auth = await _authService.Login(loginModel.Email, loginModel.Password);
            if (!auth.Status)
            {
                return ServiceResult<string>.Failure(auth.Message);
            }
            var user = await _userService.GetUserByEmail(loginModel.Email);
            var token = _tokenService.GenerateAccessToken(user.Data!);
            var checkToken = await _refreshTokenService.FindTokensUserIsNotRevokeThenPactchIsRevoke(user.Data!.Id);
            if(!checkToken.Status)
            {
                return ServiceResult<string>.Failure(checkToken.Message);
            }
            var refreshToken = new RefreshToken
            {
                UserId = user.Data.Id,
                Token = token,
                IsRevoke = false,
                CreateAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(30)
            };
            var addRefreshToken = await _refreshTokenService.AddRefreshTokenAsync(refreshToken);
            if (!addRefreshToken.Status)
            {
                return ServiceResult<string>.Failure(addRefreshToken.Message);
            }
            return ServiceResult<string>.Success(token);
        }
        public async Task<ServiceResult<string>> Register(RegisterModel model)
        {
            var register = await _authService.Register(model.Email, model.UserName, model.Password);
            if(!register.Status)
            {
                return ServiceResult<string>.Failure(register.Message);
            }
            return ServiceResult<string>.Success("User registered successfully.");
        }
        public async Task<ServiceResult<string>> LogOut(string token)
        {
            var refreshToken = await _refreshTokenService.GetRefreshTokenByToken(token);
            if(!refreshToken.Status)
            {
                return ServiceResult<string>.Failure(refreshToken.Message);
            }
            var pacthRefreshToken = await _refreshTokenService.PacthAsync(refreshToken.Data!, new Dictionary<string, object>()
            {
                {"IsRevoke", true}
            });
            if (!pacthRefreshToken.Status)
            {
                return ServiceResult<string>.Failure(pacthRefreshToken.Message);
            }
            return ServiceResult<string>.Success("Log out is success");
        }
        public async Task<ServiceResult<string>> ChangePassword(string email, string oldPassword, string newPassword)
        {
            var changePassword = await _authService.ChangePassword(email, oldPassword, newPassword);
            if (changePassword.Status == false)
            {
                return ServiceResult<string>.Failure(changePassword.Message);

            }
            return ServiceResult<string>.Success("Change password is success");
        }
    }
}
