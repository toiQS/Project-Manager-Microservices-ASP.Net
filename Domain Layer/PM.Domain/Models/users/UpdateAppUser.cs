namespace PM.Domain.Models.users
{
    public class UpdateAppUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        //public string Password { get; set; } = string.Empty;
        public string PathImage { get; set; } = string.Empty;
        public string Phone {  get; set; } = string.Empty;
    }
}
