using Microsoft.AspNetCore.Identity;
using SchoolSystem.Domain.Interfaces;
using SchoolSystem.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Infrastructure.Respositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UsuarioRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<string> GetEmailById(string usuarioId)
        {
            var user = await _userManager.FindByIdAsync(usuarioId);
            return user?.Email ?? "Sin Correo";
        }
    }
}
