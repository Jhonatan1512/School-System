using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Application.DTOs;
using SchoolSystem.Infrastructure.Identity;
using System.Security.Claims;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SchoolSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace School_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private ApplicationDbContext _context;

        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration, ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
            _roleManager = roleManager;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] RegistrarUsuarioDto modelo)
        {
            var nuevoUsuario = new ApplicationUser
            {
                UserName = modelo.Email,
                Email = modelo.Email,
                EsActivo = true
            };

            var resultado = await _userManager.CreateAsync(nuevoUsuario, modelo.Password);

            if (resultado.Succeeded)
            {
                var rolExiste = await _roleManager.RoleExistsAsync(modelo.Rol);
                if (!await _roleManager.RoleExistsAsync(modelo.Rol))
                {
                    await _roleManager.CreateAsync(new IdentityRole(modelo.Rol));
                }

                await _userManager.AddToRoleAsync(nuevoUsuario, modelo.Rol);
                return Ok(new { mensaje = "Usuario Creado y Rol Asignado" });
            }

            return BadRequest(resultado.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto modelo)
        {
            var usuario = await _userManager.FindByEmailAsync(modelo.Email);

            if (usuario == null)
            {
                return Unauthorized(new { mensaje = "El correo es incorrecto" });
            }

            var passwordCorrecat = await _userManager.CheckPasswordAsync(usuario, modelo.Password);

            if (!passwordCorrecat)
            {
                return Unauthorized(new { mensaje = "La contraseña es incorrecta" });
            }

            var roles = await _userManager.GetRolesAsync(usuario);
            string nombreToke = "Usuario";

            if (roles.Contains("Admin"))
            {
                nombreToke = "Administrador del Sistema";
            } else
            {
                var alumno = await _context.Alumnos.FirstOrDefaultAsync(a => a.UsuarioId== usuario.Id);
                if(alumno != null)
                {
                    nombreToke = $"{alumno.Nombre} {alumno.Apellidos}";
                }
                else
                {
                    var docente = await _context.Docentes.FirstOrDefaultAsync(d => d.UsuarioId == usuario.Id);
                    if(docente != null)
                    {
                        nombreToke = $"{docente.Nombres} {docente.Apellidos}";
                    }
                }
            }


            
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("nombre", nombreToke)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: creds
            );

            return Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiracion = token.ValidTo
            });
        }

    }
}
