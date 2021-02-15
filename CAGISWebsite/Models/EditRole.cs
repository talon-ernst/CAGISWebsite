using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CAGISWebsite.Models
{
    public class EditRole
    {
        public EditRole()
        {
            Users = new List<IdentityUser>();
        }

        public string Id { get; set; }
        [Required(ErrorMessage = "Role Name is required")]
        public string RoleName { get; set; }

        public List<IdentityUser> Users { get; set; }
    }
}
