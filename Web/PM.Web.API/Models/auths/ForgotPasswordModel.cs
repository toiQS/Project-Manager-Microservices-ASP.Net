namespace PM.Web.API.Models.auths
{
    public class ForgotPasswordModel
    {
        public string Email { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
