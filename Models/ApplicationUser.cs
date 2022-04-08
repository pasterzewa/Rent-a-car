using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rent_a_Car.Models
{
    
    public class ApplicationUser : IdentityUser
    {
        public int AccountType { get; set; } = 0;
    }
}
