using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<string> GetEmailById(string usuarioId);
    }
}
