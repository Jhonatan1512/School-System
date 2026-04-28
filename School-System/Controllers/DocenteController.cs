using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Application.DTOs;
using SchoolSystem.Application.Interfaces;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Interfaces;
using SchoolSystem.Infrastructure.Identity;
using SchoolSystem.Infrastructure.Respositories;
using System.Security.Claims;

namespace School_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DocenteController : ControllerBase
    {   
        private readonly IDocenteRepository _docenteRepository; 
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDocenteService _docenteService; 
        private readonly ICalificacionService _calificacionService;
        private readonly IPeriodoAcademicoRepository _periodoAcademicoRepository;
        private readonly RoleManager<IdentityRole> _roleManager;
        public DocenteController(
            IDocenteRepository docenteRepository,  
            UserManager<ApplicationUser> userManager, 
            IDocenteService docenteService, 
            ICalificacionService calificacionService,
            IPeriodoAcademicoRepository periodoAcademicoRepository,
            RoleManager<IdentityRole> roleManager)
        {
            _docenteRepository = docenteRepository;
            _userManager = userManager;
            _docenteService = docenteService;
            _calificacionService = calificacionService;
            _periodoAcademicoRepository = periodoAcademicoRepository;
            _roleManager = roleManager;
        }

        //POST :api/docente
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AgregarDocente([FromBody] DocenteDto docenteDto)
        {
            var apellidosPartes = docenteDto.Apellidos.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            char inicial1 = apellidosPartes.Length > 0 ? apellidosPartes[0][0] : 'x';
            char inicial2 = apellidosPartes.Length > 1 ? apellidosPartes[1][0] : inicial1;

            string emailGenerado = $"{char.ToLower(inicial1)}{char.ToLower(inicial2)}{docenteDto.Dni}@ejemplo.edu.pe";
            var passwordGenerada = $"{char.ToUpper(inicial1)}{char.ToLower(inicial2)}{docenteDto.Dni}*";

            var usuarioNuevo = new ApplicationUser
            {
                UserName = emailGenerado,
                Email = emailGenerado,
                EsActivo = true,
            };

            var resultadoIdentity = await _userManager.CreateAsync(usuarioNuevo, passwordGenerada);

            const string rolAlumno = "Docente";
            if (!await _roleManager.RoleExistsAsync(rolAlumno))
            {
                await _roleManager.CreateAsync(new IdentityRole(rolAlumno));
            }

            await _userManager.AddToRoleAsync(usuarioNuevo, rolAlumno);

            if (!resultadoIdentity.Succeeded)
            {
                return BadRequest(resultadoIdentity.Errors);
            }

            var nuevoDocente = new Docente
            {
                Nombres = docenteDto.Nombres,
                Apellidos = docenteDto.Apellidos,
                Dni = docenteDto.Dni,
                UsuarioId = usuarioNuevo.Id,
            };

            var docenteCreado = await _docenteRepository.CrearDocenteAsync(nuevoDocente);

            return Ok(new
            {
                mensaje = "Docente Creado",
                Credenciales = new
                {
                    Email = emailGenerado,
                    Passoword = passwordGenerada,
                },
                Docente = docenteCreado
            });
        }

        //GET :api/docente
        [HttpGet("lista-docentes")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> obtenerTodos([FromQuery] int pagina = 1, [FromQuery] int cantidad = 10)
        {
            if(pagina < 1) pagina = 1;
            if(cantidad > 20) cantidad = 20;
            try
            {
                var docentes = await _docenteService.GetAllsync(pagina, cantidad);
                return Ok(docentes);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //GET :api:docente
        [HttpGet("dni/{dni}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ObtenerPorDni(string dni)
        {
            var docente = await _docenteService.GetByDniAsync(dni);

            if (docente == null)
            {
                return NotFound(new { mensaje = "Docente no encontrado" });
            }
            return Ok(docente);
        }

        //PATCH :api/docente/dni
        [HttpPatch("{dni}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ResetPasword(string dni, [FromBody] AdminResetPassword adminResetPassword)
        {
            var docente = await _docenteRepository.ObtenerPorDniAsync(dni);
            if (docente == null) return NotFound(new { mensaje = "Docente no encontrado" });

            var usuario = await _userManager.FindByIdAsync(docente.UsuarioId);
            if (usuario == null) return NotFound(new { mensaje = "Usuario no encotrado" });

            var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);

            var resultado = await _userManager.ResetPasswordAsync(usuario, token, adminResetPassword.NuevaPassword);
            if (resultado == null) return BadRequest(resultado!.Errors);

            return Ok(new
            {
                mensaje = "Contraseña Actualizada",
                neuvaPassword = adminResetPassword.NuevaPassword
            });
        }

        //PUT :api/doncente/password
        [HttpPut("password")]
        [Authorize(Roles = "Docente")]
        public async Task<IActionResult> ResetPasswordDocente([FromBody] UserResetPasswordDto userResetPassword)
        {
            var usuarioLogueado = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (usuarioLogueado == null) return Unauthorized("Token invalido o no esta autenticado");

            var usuarioExiste = await _userManager.FindByIdAsync(usuarioLogueado);
            if (usuarioExiste == null) return NotFound(new { mensaje = "Usuario no encontrado" });

            var resultado = await _userManager.ChangePasswordAsync(usuarioExiste, userResetPassword.PasswordActual, userResetPassword.NuevaPassword);
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

        //PUT :api/docente/dni
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ModificarDocente(int id, [FromBody] DocenteDto dto)
        {
            var docente = await _docenteRepository.ObtenerPorId(id);
            if (docente == null) return NotFound(new { mensaje = "Docente no encontrado" });

            var partesApellidos = dto.Apellidos.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            char inicial1 = partesApellidos.Length > 0 ? partesApellidos[0][0] : 'x';
            char inicial2 = partesApellidos.Length > 1 ? partesApellidos[1][0] : inicial1;

            string emailNuevo = $"{char.ToLower(inicial1)}{char.ToLower(inicial2)}{dto.Dni}@ejemplo.edu.pe";
            var passwordNueva = $"{char.ToUpper(inicial1)}{char.ToLower(inicial2)}{dto.Dni}*";

            var usuario = await _userManager.FindByIdAsync(docente.UsuarioId);

            if (usuario != null)
            {
                usuario.UserName = emailNuevo;
                usuario.Email = emailNuevo;

                await _userManager.UpdateAsync(usuario);

                var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);
                var resultadoPassword = await _userManager.ResetPasswordAsync(usuario, token, passwordNueva);

                if (!resultadoPassword.Succeeded) return BadRequest(resultadoPassword.Errors);
            };

            docente.Nombres = dto.Nombres;
            docente.Apellidos = dto.Apellidos;
            docente.Dni = dto.Dni;

            await _docenteRepository.ActualizarDoncenteAsync(docente);

            var AlumnoActualizadoCompleto = await _docenteService.GetByDniAsync(docente.Dni);

            return Ok(new
            {
                mensaje = "Datos del docente Actualizados",
                Credenciales = new
                {
                    Email = emailNuevo,
                    Passoword = passwordNueva,
                },
                Docente = AlumnoActualizadoCompleto
            });
        }

        [HttpGet("dashboard")]
        [Authorize(Roles = "Docente")]
        public async Task<IActionResult> ObtenerMiDashboard()
        {
            try
            {
                var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(usuarioId)) return Unauthorized();

                var dashboard = await _docenteService.ObtenerMiDashboardAsync(usuarioId);

                if (!dashboard.Any())
                {
                    return Ok(new
                    {
                        mensaje = "Sin Cursos Asignados en el Periodo Actual",
                        data = dashboard
                    });
                }
                return Ok(new
                {
                    mensaje = "Cursos Asignados",
                    data = dashboard
                }); 
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("notas")]
        [Authorize(Roles = "Docente")]
        public async Task<IActionResult> LlenarNotas(int docenteId, [FromBody] List<CalificacionCreateDto> dto)
        {
            try
            {
                var docenteIdClaims = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(docenteIdClaims)) return Unauthorized("No se pudo identificar al docente en el token");

                var docente = await _docenteRepository.ObtenerPorUsuarioAsync(docenteIdClaims);
                if (docente == null) return Forbid("El usuario actual no tiene un perfil de docente asignado o esta inactivo");

                //int docenteId = int.Parse(docenteIdClaims);

                var resultado = await _calificacionService.RegistroMasivo(docente.Id, dto);
                return Ok(new
                {
                    mesanje = "Nota agrega",
                    data = resultado
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("curso/{cursoId}/seccion/{seccionId}/detalle")]
        [Authorize(Roles = "Docente")]
        public async Task<IActionResult> ObtenerDetallesCurso(int cursoId, int seccionId)
        {
            try
            {
                var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(usuarioId))
                    return Unauthorized("No se encontro el token del usuario");

                var docente = await _docenteRepository.ObtenerPorUsuarioAsync(usuarioId);
                if (docente == null) return BadRequest("El perfil del docente no existe");

                var periodoActivo = await _periodoAcademicoRepository.ObtenerPeriodoAcademicoActivo();
                if (periodoActivo == null) return BadRequest("El periodo académico no esta activo actualmente");

                var detalleCurso = await _docenteService.ObtenerDetalleCursoAsync(docente.Id, cursoId, seccionId, periodoActivo.Id);    

                return Ok(detalleCurso);

            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("docenteId/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ModificarEstado(int id, ActualizarEstadoDocenteDto dto)
        {
            try
            {
                await _docenteService.ActualizarEstadoAsync(id, dto);
                return Ok(new {mensaje = "Estado del docente actualizado"});
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("perfil-docente")]
        [Authorize(Roles = "Docente")]
        public async Task<ActionResult> GetPerfil()
        {
            try
            {
                var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if(string.IsNullOrEmpty(usuarioId))
                {
                    return NotFound();  
                }

                var perfil = await _docenteService.GetPerfilAsyn(usuarioId);
                if(perfil == null)
                {
                    return NotFound("Perfil no encontrado");
                }
                return Ok(perfil);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}