namespace PM.Web.API.Models.users
{
    public class UserPatchModel
    {
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string AvataPath { get; set; } = string.Empty;
    }
}
