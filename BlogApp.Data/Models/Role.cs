using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Data.Models
{
    public class Role
    {
        public long Id { get; set; }

        public string RoleName { get; set; }

        public List<User> Users { get; set; }
    }
}
