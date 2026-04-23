using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Application.DTOs;
using SchoolSystem.Application.Interfaces;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Interfaces;

namespace School_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class CursoController : ControllerBase
    {
        private readonly ICursoRepository _cursoRepository; 
        private readonly IGradoRepository _gradoRepository;
        private readonly ICursoService _cursoService;
        private readonly IPeriodoAcademicoRepository _periodoAcademicoRepository;
        public CursoController(ICursoRepository cursoRepository, IGradoRepository gradoRepository, ICursoService cursoService, IPeriodoAcademicoRepository periodoAcademicoRepository)
        {
            _cursoRepository = cursoRepository;
            _gradoRepository = gradoRepository;
            _cursoService = cursoService;
            _periodoAcademicoRepository = periodoAcademicoRepository;
        }

        //POST :api/curso
        [HttpPost]
        public async Task<IActionResult> Crearcurso([FromBody] CursoCompetenciaDto curso)
        {
            try
            {
                var cursoCreado = await _cursoService.CrearCursoCompetenciaAsync(curso);
                return Ok(new
                {
                    mensaje = "Curso y Competencias creados",
                    cursoId = cursoCreado.Id,
                    nombre = cursoCreado.Nombre,
                    totalCompetencia = cursoCreado.Competencias.Count(),
                });
            }
            catch (Exception ex) 
            {
                return BadRequest(ex);
            }
        } 

        //GET :api/curso
        [HttpGet("lista-cursos")]
        public async Task<IActionResult> ObtenerCursos([FromQuery] int pagina = 1, [FromQuery] int cantidad = 10)
        {
            if (pagina < 1) pagina = 1;
            if (cantidad > 20) cantidad = 20;

            var cursos = await _cursoService.ObtenerTodosAsync(pagina, cantidad);
            return Ok(cursos);
        }

        //PUT :api/curso
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarCurso(int id, [FromBody] CursoCompetenciaDto dto)
        {
            try
            {
                var resultado = await _cursoService.ActualizarCursoCompetenciaAsync(id, dto);

                if (!resultado) return NotFound(new {mensaje = "Curso No Encontrado"});

                return Ok(new { mesaje = "Datos del Curso Actualizados", datos = resultado});
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            try
            {
                var resultado = await _cursoService.ObtenerPorIdAsync(id);
                return Ok(new { mensaje = "Competencias del Curso", data = resultado });
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("cursoId/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ActualizarCurso(int id, CursoActualizarDto dto)
        {
            try
            {
                await _cursoService.ActualuzarAsync(id, dto);
                return Ok(new { mensaje = "Curso actualizado" });
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("gradoId/{gradoId:int}/seccionId/{seccionId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetByGradoSeccion(int gradoId, int seccionId)
        {
            var periodActivo = await _periodoAcademicoRepository.ObtenerPeriodoAcademicoActivo();
            try
            {
                var result = await _cursoService.ObtenerPorGradoSeccionAsyn(gradoId, seccionId, periodActivo!.Id); 
                return Ok(result);
            } catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("gradoId/{gradoId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetByGrado(int gradoId)
        {
            var periodActivo = await _periodoAcademicoRepository.ObtenerPeriodoAcademicoActivo();
            try
            {
                var result = await _cursoService.ObtenerPorGrado(gradoId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
