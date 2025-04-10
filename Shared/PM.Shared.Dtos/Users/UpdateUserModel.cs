namespace PM.Shared.Dtos.Users
{
    public class UpdateUserModel
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string AvatarPath { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
    }
}
