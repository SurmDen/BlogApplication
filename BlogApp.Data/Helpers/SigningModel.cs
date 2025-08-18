using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Data.Helpers
{
    public class SigningModel
    {
        public SigningModel(string name, string email, string role)
        {
            UserEmail = email;
            UserName = name;
            UserRole = role;
        }

        public SigningModel()
        {
            
        }

        public string UserName { get; set; }

        public string UserEmail { get; set; }

        public string UserRole { get; set; }
    }
}
