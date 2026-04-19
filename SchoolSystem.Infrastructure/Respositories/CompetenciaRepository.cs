using Microsoft.EntityFrameworkCore;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Interfaces;
using SchoolSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Infrastructure.Respositories
{
    public class CompetenciaRepository : ICompetenciaRepository
    {
        private readonly ApplicationDbContext context;

        public CompetenciaRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<Competencia?> BuscarPorIdAsync(int id)
        {
            return await context.Competencias.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task CrearCompetenciaAsync(Competencia competencia)
        {
            await context.Competencias.AddAsync(competencia);
            await context.SaveChangesAsync();
        }

        public async Task EditarCompetenciaAsync(Competencia competencia)
        {
            context.Competencias.Update(competencia);
            await context.SaveChangesAsync();
        }

        public async Task EliminarCompetenciaAsync(int id)
        {
            await context.Competencias.Where(c => c.Id == id).ExecuteDeleteAsync();
            await context.SaveChangesAsync();
        }
    }
}
