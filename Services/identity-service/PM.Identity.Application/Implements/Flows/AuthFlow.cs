using PM.Identity.Application.Interfaces.Flows;
using PM.Identity.Application.Interfaces.Services;
using PM.Identity.Domain.Entities;
using PM.Shared.Dtos.Auths;
using PM.Shared.Dtos;

public class AuthFlow : IAuthFlow
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenService _refreshTokenService;

    public AuthFlow(
        IAuthService authService,
        IUserService userService,
        ITokenService tokenService,
        IRefreshTokenService refreshTokenService)
    {
        _authService = authService;
        _userService = userService;
        _tokenService = tokenService;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<ServiceResult<string>> HandleSignInAsync(LoginModel loginModel)
    {
        var auth = await _authService.SignInAsync(loginModel.Email, loginModel.Password);
        if (!auth.Status)
            return ServiceResult<string>.Failure(auth.Message);

        var user = await _userService.GetUserByEmail(loginModel.Email);
        var token = _tokenService.GenerateAccessToken(user.Data!);

        var revokeOldTokens = await _refreshTokenService.RevokeAllActiveTokensByUserIdAsync(user.Data!.Id);
        if (!revokeOldTokens.Status)
            return ServiceResult<string>.Failure(revokeOldTokens.Message);

        var refreshToken = new RefreshToken
        {
            UserId = user.Data.Id,
            Token = token,
            IsRevoke = false,
            CreateAt = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(30)
        };

        var addToken = await _refreshTokenService.CreateRefreshTokenAsync(refreshToken);
        if (!addToken.Status)
            return ServiceResult<string>.Failure(addToken.Message);

        return ServiceResult<string>.Success(token);
    }

    public async Task<ServiceResult<string>> HandleRegisterUserAsync(RegisterModel model)
    {
        var result = await _authService.RegisterUserAsync(model.Email, model.UserName, model.Password);
        return result.Status
            ? ServiceResult<string>.Success("User registered successfully.")
            : ServiceResult<string>.Failure(result.Message);
    }

    public async Task<ServiceResult<string>> HandleSignOutAsync(string token)
    {
        var refreshToken = await _refreshTokenService.GetRefreshTokenByToken(token);
        if (!refreshToken.Status)
            return ServiceResult<string>.Failure(refreshToken.Message);

        var revoke = await _refreshTokenService.UpdateTokenFieldsAsync(refreshToken.Data!, new Dictionary<string, object>
        {
            { "IsRevoke", true }
        });

        return revoke.Status
            ? ServiceResult<string>.Success("Sign out successful.")
            : ServiceResult<string>.Failure(revoke.Message);
    }

    public async Task<ServiceResult<string>> HandleChangePasswordAsync(ChangePassword model)
    {
        var result = await _authService.ChangeUserPasswordAsync(model.Email, model.OldPassword, model.NewPassword);
        return result.Status
            ? ServiceResult<string>.Success("Password changed successfully.")
            : ServiceResult<string>.Failure(result.Message);
    }
}
