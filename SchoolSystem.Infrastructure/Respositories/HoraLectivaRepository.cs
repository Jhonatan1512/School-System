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
    public class HoraLectivaRepository : IHoraLectivaRepository
    {
        private readonly ApplicationDbContext _context;

        public HoraLectivaRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<HoraLectiva>> GetAllAsync()
        {
            return await _context.HorasLectivas.ToListAsync();
        }
    }
}
