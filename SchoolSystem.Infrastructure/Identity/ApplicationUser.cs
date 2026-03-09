using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace SchoolSystem.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public bool EsActivo { get; set; } = true;
    }
}
