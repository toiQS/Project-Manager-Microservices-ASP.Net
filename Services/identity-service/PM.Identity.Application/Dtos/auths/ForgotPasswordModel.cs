namespace PM.Identity.Application.Dtos.auths
{
    public class ForgotPasswordModel
    {
        public string Email { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
