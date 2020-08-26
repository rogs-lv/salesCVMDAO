using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace salesCVM.Models
{
    public class User
    {
        public string IdUser { get; set; }
        public string NameUser { get; set; }
        public string IdUserSap { get; set; }
        public string Password { get; set; }
        public Rol Role { get; set; }
        public string Token { get; set; }
        public User(string _idUser, string _password) {
            this.IdUser = _idUser;
            this.Password = _password;
            this.NameUser = "";
            this.IdUserSap = "";
            this.Role = new Rol() { IdRol = 1, NameRol = "Role 1" };
            this.Token = "";
        }
    }

    public class UserLogin {
        public string IdUser { get; set; }
        public string Password { get; set; }
    }
}
