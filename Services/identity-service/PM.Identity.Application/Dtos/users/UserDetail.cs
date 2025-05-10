namespace PM.Identity.Application.Dtos.auths
{
    namespace PM.Shared.Dtos.users
    {
        public class UserDetail
        {
            public string Id { get; set; } = string.Empty;
            public string FullName { get; set; } = string.Empty;
            public string UserName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
            public string AvataPath { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
        }
    }
}
