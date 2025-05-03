namespace PM.Shared.Dtos.auths
{
    public class ChangePasswordModel
    {
        public string Email { get; set; } = string.Empty;
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
