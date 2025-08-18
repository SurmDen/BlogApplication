using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Data.Helpers
{
    public class UserShortData
    {
        public UserShortData()
        {
            
        }

        public UserShortData(long id, string name, string email)
        {
            Id = id; 
            Name = name; 
            Email = email;
        }

        public long Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
    }
}
