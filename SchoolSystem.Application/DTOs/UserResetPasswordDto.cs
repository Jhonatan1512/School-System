using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Application.DTOs
{
    public class UserResetPasswordDto
    {
        public string PasswordActual {  get; set; } = string.Empty;
        public string NuevaPassword {  get; set; } = string.Empty ;
    }
}
