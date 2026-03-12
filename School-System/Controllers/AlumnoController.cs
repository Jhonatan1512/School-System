using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Application.DTOs;
using SchoolSystem.Application.Interfaces;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Interfaces;
using SchoolSystem.Infrastructure.Identity;
using System.Security.Claims; 

namespace School_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AlumnoController : ControllerBase
    {
        private readonly IAlumnoRespository _alumnoRespository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAlumnoService _alumnoService;
        private readonly IPeriodoAcademicoRepository _periodoAcademicoRepository;

        public AlumnoController(
            IAlumnoRespository alumnoRespository, 
            UserManager<ApplicationUser> userManager, 
            IAlumnoService alumnoService,
            IPeriodoAcademicoRepository periodoAcademicoRepository)
        {
            _alumnoRespository = alumnoRespository;
            _userManager = userManager;
            _alumnoService = alumnoService;
            _periodoAcademicoRepository = periodoAcademicoRepository;
        }

        //GET :api/alumno
        [HttpGet]
        public async Task<ActionResult> obtenerTodos()
        {
            var alumnos = await _alumnoService.GetAll();
            return Ok(alumnos);
        }

        //POST :api/alumno
        [HttpPost]
        public async Task<IActionResult> CrearAlumno([FromBody] AlumnoDto alumnoDto)
        {
            var partesApellidos = alumnoDto.Apellidos.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            char inicial1 = partesApellidos.Length > 0 ? partesApellidos[0][0] : 'x';
            char inicial2 = partesApellidos.Length > 1 ? partesApellidos[1][0] : inicial1;

            string emailGenerado = $"{char.ToLower(inicial1)}{char.ToLower(inicial2)}{alumnoDto.Dni}@ejemplo.edu.pe";
            var passwordGenerada = $"{char.ToUpper(inicial1)}{char.ToLower(inicial2)}{alumnoDto.Dni}*";


            var usuarioNuevo = new ApplicationUser
            {
                UserName = emailGenerado,
                Email = emailGenerado,
                EsActivo = true,
            };

            var resultadoIdentity = await _userManager.CreateAsync(usuarioNuevo, passwordGenerada);

            if (!resultadoIdentity.Succeeded)
            {
                return BadRequest(resultadoIdentity.Errors);
            }

            var nuevoAlumno = new Alumno
            {
                Nombre = alumnoDto.Nombre,
                Apellidos = alumnoDto.Apellidos,
                Dni = alumnoDto.Dni,
                FechaNacimiento = alumnoDto.FechaNacimiento,
                Sexo = alumnoDto.Sexo,
                UsuarioId = usuarioNuevo.Id,
            };

            var alumnoCreado = await _alumnoRespository.AgregarAlumnoAsync(nuevoAlumno);

            return Ok(new
            {
                mensaje = "Alumno Creado",
                Credenciales = new
                {
                    Email = emailGenerado,
                    Passoword = passwordGenerada,
                },
                Alumno = alumnoCreado
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var alumno = await _alumnoService.GetByIdAsync(id);

            if (alumno == null)
            {
                return NotFound(new { mensaje = "Alumno no encontrado" });
            }

            return Ok(alumno);
        }
        [HttpGet("dni/{dni}")]
        public async Task<IActionResult> ObtenerPorDni(string dni)
        {
            var alumno = await _alumnoService.GetByDniAsync(dni);

            if (alumno == null)
            {
                return NotFound(new { mensaje = "Alumno no encontrado" });
            }

            return Ok(alumno);
        }

        [HttpPut("{dni}")]
        public async Task<ActionResult> ActualizarAlumno(string dni, [FromBody] AlumnoDto alumnoDto)
        {
            var alumnoExiste = await _alumnoRespository.ObtenerPorDni(dni);
            if (alumnoExiste == null) NotFound(new { mensaje = "Alumno no encontrado" });

            var partesApellidos = alumnoDto.Apellidos.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            char inicial1 = partesApellidos.Length > 0 ? partesApellidos[0][0] : 'x';
            char inicial2 = partesApellidos.Length > 1 ? partesApellidos[1][0] : inicial1;

            string emailNuevo = $"{char.ToLower(inicial1)}{char.ToLower(inicial2)}{alumnoDto.Dni}@ejemplo.edu.pe";
            var passwordNueva = $"{char.ToUpper(inicial1)}{char.ToLower(inicial2)}{alumnoDto.Dni}*";

            var usuario = await _userManager.FindByIdAsync(alumnoExiste.UsuarioId);

            if (usuario != null)
            {
                usuario.UserName = emailNuevo;
                usuario.Email = emailNuevo;

                await _userManager.UpdateAsync(usuario);

                var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);
                var resultadoPassword = await _userManager.ResetPasswordAsync(usuario, token, passwordNueva);

                if (!resultadoPassword.Succeeded) return BadRequest(resultadoPassword.Errors);
            }
            ;

            alumnoExiste.Nombre = alumnoDto.Nombre;
            alumnoExiste.Apellidos = alumnoDto.Apellidos;
            alumnoExiste.Dni = alumnoDto.Dni;
            alumnoExiste.FechaNacimiento = alumnoDto.FechaNacimiento;
            alumnoExiste.Sexo = alumnoDto.Sexo;

            await _alumnoRespository.ActualizarAlumnoAsync(alumnoExiste);

            var AlumnoActualizadoCompleto = await _alumnoService.GetByDniAsync(alumnoExiste.Dni);

            return Ok(new
            {
                mensaje = "Datos del Alumno Actualizados",
                Credenciales = new
                {
                    Email = emailNuevo,
                    Passoword = passwordNueva,
                },
                Alumno = AlumnoActualizadoCompleto
            });
        }

        [HttpPatch("{dni}")]
        public async Task<IActionResult> ResetearPasswordAdmin(string dni, [FromBody] AdminResetPassword adminResetPassword)
        {
            var alumnoExistente = await _alumnoRespository.ObtenerPorDni(dni);
            if (alumnoExistente == null) return NotFound(new { mensaje = "Alumno no encontrado" });

            var usuario = await _userManager.FindByIdAsync(alumnoExistente.UsuarioId);
            if (usuario == null) return NotFound(new { mensaje = "Usuario no encontrado" });

            var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);

            var resultado = await _userManager.ResetPasswordAsync(usuario, token, adminResetPassword.NuevaPassword);
            if (resultado == null) return BadRequest(resultado.Errors);

            return Ok(new
            {
                mensaje = "Contraseña Actualizada",
                neuvaPassword = adminResetPassword.NuevaPassword
            });
        }

        [HttpPut("passowrd")]
        [Authorize]
        public async Task<IActionResult> CambiarMiPassword([FromBody] UserResetPasswordDto alumnoResetPasswordDto)
        {
            var usuarioLogueadoId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (usuarioLogueadoId == null)
            {
                return Unauthorized(new { mensaje = " Token invalido o no esta autenticado" });
            }

            var usuarioExiste = await _userManager.FindByIdAsync(usuarioLogueadoId);
            if (usuarioExiste == null) return NotFound(new { mensaje = "Usuario no encontrado" });

            var resultado = await _userManager.ChangePasswordAsync(usuarioExiste, alumnoResetPasswordDto.PasswordActual, alumnoResetPasswordDto.NuevaPassword);
            if (!resultado.Succeeded)
            {
                var errorPassword = resultado.Errors.FirstOrDefault(e => e.Code == "PasswordMismatch");
                if (errorPassword != null)
                {
                    return BadRequest(new { mensaje = "La contraseña actual es incorrecta" });
                }
                return BadRequest(new
                {
                    mensaje = "La contraseña no cumple con los requisitos Obligatorios",
                    detalles = resultado.Errors.Select(e => e.Description)
                });
            }

            return Ok(new { mensaje = "Contraseña actualizada correctamente" });
        }

        [HttpGet("mis-cursos")]
        public async Task<IActionResult> ObtenrMisCursos()
        {
            try
            {
                var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(usuarioId)) return Unauthorized(new { mensaje = "Token Invalido o Usuario no Autenticado" });

                var dashboard = await _alumnoService.ObtenerMisCursos(usuarioId);

                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("mis-cursos/{cursoId}/detalle")]
        public async Task<IActionResult> ObtenerDetalleCursos(int cursoId)
        {
            try
            {
                var usuarioId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var alumno = await _alumnoRespository.ObtenerPorUsuarioAsync(usuarioId);
                if (alumno == null) return Unauthorized("Perfil de Alumno no encontrado");

                var periodoActivo = await _periodoAcademicoRepository.ObtenerPeriodoAcademicoActivo();
                if (periodoActivo == null) return BadRequest("No existe periodo académico activo");

                var detalle = await _alumnoService.ObtenerDetalleCursoAsync(alumno.Id, cursoId, periodoActivo.Id);
                return Ok(detalle);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
