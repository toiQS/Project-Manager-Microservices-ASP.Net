namespace PM.EndPointAPIs.Model.Auth.Commands
{
    public class RegisterCommand
    {
        public string Username { get; set; } = string.Empty;
        public string Email {  get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
