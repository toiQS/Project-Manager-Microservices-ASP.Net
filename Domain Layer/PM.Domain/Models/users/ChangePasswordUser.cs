namespace PM.Domain.Models.users
{
    class ChangePasswordUser
    {
        public string Email { get; set; } = string.Empty;
        public string OldPassword { get; set; } = string.Empty; 
        public string NewPassword { get; set; } = string.Empty;
    }
}
