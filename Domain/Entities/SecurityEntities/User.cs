global using Microsoft.AspNetCore.Identity;
//using Domain.Entities.DoctorEntites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.SecurityEntities
{
    public enum Role : byte
    {
        Admin = 1,
        Manager = 2,
        Editor = 3
    }
    public enum Gender : byte
    {
        Unspecified = 0,
        Male = 1,
        Female = 2
    }
    public class User : IdentityUser
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public Gender Gender { get; set; }
        public Role UserRole { get; set; }


    }
   
}
