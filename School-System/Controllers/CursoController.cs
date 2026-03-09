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
        public CursoController(ICursoRepository cursoRepository, IGradoRepository gradoRepository, ICursoService cursoService)
        {
            _cursoRepository = cursoRepository;
            _gradoRepository = gradoRepository;
            _cursoService = cursoService;
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
        [HttpGet]
        public async Task<IActionResult> ObtenerCursos()
        {
            var cursos = await _cursoService.ObtenerTodosAsync();
            return Ok(cursos);
        }

        //PUT :api/curso
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarCurso(int id, [FromBody] CursoCompetenciaDto dto)
        {
            try
            {
                var resultado = await _cursoService.ActualizarCursoCompetenciaAsync(id, dto);

                if (!resultado) return NotFound(new {mensaje = "Cursi No Encontrado"});

                return Ok(new { mesaje = "Datos del Curso Actualizados", datos = resultado});
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
