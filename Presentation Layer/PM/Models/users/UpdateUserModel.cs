using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Models.users
{
public    class UpdateUserModel
    {
        public string Token { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        //public string Password { get; set; } = string.Empty;
        public string PathImage { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}
