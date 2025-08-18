using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Data.Models
{
    public class User
    {
        public long Id { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public List<Blog> Blogs { get; set; } = new List<Blog>();

        public Role? Role { get; set; }

        public long RoleId { get; set; }
    }
}
