using PM.Identity.Application.Interfaces.Services;

namespace PM.Identity.Application.Implements.Flows
{
    public class UserFlow
    {
        private readonly IUserService _userService;
        public UserFlow(IUserService userService)
        {
            _userService = userService;
        }
        //public string 
    }
}
