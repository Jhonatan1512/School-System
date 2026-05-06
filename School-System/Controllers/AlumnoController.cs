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
        private readonly RoleManager<IdentityRole> _roleManager; 
        private readonly IGradoRepository _gradoRepository;
        private readonly ISeccionRepository _seccionRepository;
        public AlumnoController( 
            IAlumnoRespository alumnoRespository, 
            UserManager<ApplicationUser> userManager, 
            IAlumnoService alumnoService,
            IPeriodoAcademicoRepository periodoAcademicoRepository,
            RoleManager<IdentityRole> roleManager,
            IGradoRepository gradoRepository,
            ISeccionRepository seccionRepository)
        {
            _alumnoRespository = alumnoRespository;
            _userManager = userManager;
            _alumnoService = alumnoService;
            _periodoAcademicoRepository = periodoAcademicoRepository;
            _roleManager = roleManager;
            _gradoRepository = gradoRepository;
            _seccionRepository = seccionRepository;
        }

        //GET :api/alumno
        [HttpGet("lista-paginada")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PageResponseDto<AlumnoDto>>> obtenerTodos([FromQuery] int pagina = 1, [FromQuery] int cantidad = 10)
        {
            if(pagina < 1) pagina = 1;
            if(cantidad > 20) cantidad = 20;
            var alumnos = await _alumnoService.GetAll(pagina, cantidad); 
            return Ok(alumnos);  
        }

        [HttpGet("ultima-Matricula/{id:int}")]
        [Authorize(Roles = "Alumno")]
        public async Task<IActionResult> UltimaMatricula(int id)
        {
            try
            {
                var result = await _alumnoService.UltimaMatricula(id);
                if(result == null)
                {
                    return NoContent();
                }

                return Ok(result);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //POST :api/alumno
        [HttpPost]
        [Authorize(Roles = "Admin")]
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

            const string rolAlumno = "Alumno";
            if(!await _roleManager.RoleExistsAsync(rolAlumno))
            {
                await _roleManager.CreateAsync(new IdentityRole(rolAlumno));
            }

            await _userManager.AddToRoleAsync(usuarioNuevo, rolAlumno);

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
                    Password = passwordGenerada,
                },
                Alumno = alumnoCreado
            });
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ObtenerPorDni(string dni)
        {
            var alumno = await _alumnoService.GetByDniAsync(dni);

            if (alumno == null)
            {
                return NotFound(new { mensaje = "Alumno no encontrado" });
            }

            return Ok(alumno);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ActualizarAlumno(int id, [FromBody] AlumnoDto alumnoDto)
        {
            var alumnoExiste = await _alumnoRespository.GetById(id);
            if (alumnoExiste == null) NotFound(new { mensaje = "Alumno no encontrado" });

            var otroConDni = await _alumnoRespository.ObtenerPorDni(alumnoDto.Dni);
            if(otroConDni != null && otroConDni.Id != id)
            {
                return BadRequest("El DNI ya esta registrado en la BD");
            }

            var partesApellidos = alumnoDto.Apellidos.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            char inicial1 = partesApellidos.Length > 0 ? partesApellidos[0][0] : 'x';
            char inicial2 = partesApellidos.Length > 1 ? partesApellidos[1][0] : inicial1;

            string emailNuevo = $"{char.ToLower(inicial1)}{char.ToLower(inicial2)}{alumnoDto.Dni}@ejemplo.edu.pe";
            var passwordNueva = $"{char.ToUpper(inicial1)}{char.ToLower(inicial2)}{alumnoDto.Dni}*";

            var usuario = await _userManager.FindByIdAsync(alumnoExiste!.UsuarioId);
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
                    Password = passwordNueva, 
                },
                Alumno = AlumnoActualizadoCompleto
            });
        }

        [HttpPatch("{dni}")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> ResetearPasswordAdmin(string dni, [FromBody] AdminResetPassword adminResetPassword)
        {
            var alumnoExistente = await _alumnoRespository.ObtenerPorDni(dni);
            if (alumnoExistente == null) return NotFound(new { mensaje = "Alumno no encontrado" });

            var usuario = await _userManager.FindByIdAsync(alumnoExistente.UsuarioId); 
            if (usuario == null) return NotFound(new { mensaje = "Usuario no encontrado" });

            var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);

            var resultado = await _userManager.ResetPasswordAsync(usuario, token, adminResetPassword.NuevaPassword);
            if (!resultado.Succeeded)
            {
                return BadRequest(new
                {
                    mensaje = "Error al Actualizar la contraseña",
                    errors = resultado.Errors
                });
            }

            return Ok(new
            {
                mensaje = "Contraseña Actualizada",
                nuevaPassword = adminResetPassword.NuevaPassword
            });
        }

        [HttpPut("password")]
        [Authorize(Roles = "Alumno")]
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
        [Authorize(Roles = "Alumno")]
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
        [Authorize(Roles = "Alumno")]
        public async Task<IActionResult> ObtenerDetalleCursos(int cursoId)
        {
            try
            {
                var usuarioId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
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

        [HttpGet("gradoId/{gradoId}/seccionId/{seccionId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ObtenerAlumnosSeccion(int gradoId, int seccionId)
        {
            try
            {
                var periodo = await _periodoAcademicoRepository.ObtenerPeriodoAcademicoActivo();
                if (periodo == null) return BadRequest("No existe periodo activo");

                var grado = await _gradoRepository.ObtenerPorId(gradoId);
                if (grado == null) return BadRequest("Grado no encontrado)");

                var seccion = await _seccionRepository.ObtenerPorIdAsync(seccionId);
                if (seccion == null) return BadRequest("Sección no encontrada");

                var detalle = await _alumnoService.AlumnosSeccionAsync(grado.Id, seccion.Id, periodo.Id);

                if (detalle == null || !detalle.Any())
                {
                    return Ok(new { 
                        mensaje = "No hay alumnos matriculados en esta sección en el periodo actual",
                        data = new List<AlumnoDto>()});
                } else
                {
                    return Ok(detalle);
                }

            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("alumnoId/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateEstado(int id, ActualizarEstadoDto dto)
        {
            try
            {
                var alumnoExiste = await _alumnoRespository.GetById(id);
                if (alumnoExiste == null) return NotFound("Alumno no encontrado");

                await _alumnoService.ActualizarEstadoAsync(id, dto);
                return Ok(new { mensaje = "Estado del alumno actualizado"});
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("perfil-alumno")]
        [Authorize(Roles = "Alumno")]
        public async Task<IActionResult> GetPerfil()
        {
            try
            {
                var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrWhiteSpace(usuarioId))
                {
                    return NotFound();
                }

                var perfil = await _alumnoService.GetPerfilAsync(usuarioId);
                return Ok(perfil);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
