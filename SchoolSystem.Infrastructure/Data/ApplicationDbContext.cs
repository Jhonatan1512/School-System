using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Infrastructure.Identity;



namespace SchoolSystem.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Alumno> Alumnos => Set<Alumno>();
        public DbSet<Docente> Docentes => Set<Docente>();
        public DbSet<PeriodoAcademico> PeriodoAcademicos => Set<PeriodoAcademico>();
        public DbSet<Grado> Grados => Set<Grado>();
        public DbSet<Seccion> Secciones => Set<Seccion>();

        public DbSet<Curso> Cursos => Set<Curso>();
        public DbSet<Competencia> Competencias => Set<Competencia>();

        public DbSet<Matricula> Matriculas => Set<Matricula>();
        public DbSet<AsignacionDocente> AsignacionDocentes => Set<AsignacionDocente>();
        public DbSet<Calificacion> Calificaciones => Set<Calificacion>();
        public DbSet<DetalleMatricula> DetalleMatriculas => Set<DetalleMatricula>();

        public DbSet<Horario> Horario => Set<Horario>();    

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Alumno>().HasIndex(a => a.Dni).IsUnique();

            builder.Entity<Calificacion>().Property(c => c.Nota).HasMaxLength(2);

            builder.Entity<Matricula>().HasOne(m => m.PeriodoAcademico).WithMany().HasForeignKey(m => m.PeriodoAcademicoId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Matricula>().HasOne(m => m.Grado).WithMany().HasForeignKey(m => m.GradoId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Matricula>().HasOne(m => m.Seccion).WithMany().HasForeignKey(m => m.SeccionId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AsignacionDocente>().HasOne(a => a.Curso).WithMany().HasForeignKey(a => a.CursoId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AsignacionDocente>().HasOne(a => a.Seccion).WithMany().HasForeignKey(a => a.SeccionId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AsignacionDocente>().HasOne(a => a.PeriodoAcademico).WithMany().HasForeignKey(a => a.PeriodoAcademicoId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Docente>().HasOne<ApplicationUser>().WithMany().HasForeignKey(e => e.UsuarioId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Alumno>().HasOne<ApplicationUser>().WithMany().HasForeignKey(e => e.UsuarioId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Calificacion>().HasOne(c => c.DetalleMatricula).WithMany(d => d.Calificaciones).HasForeignKey(c => c.DetalleMatriculaId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Calificacion>().HasOne(c => c.Competencia).WithMany().HasForeignKey(c => c.CompetenciaId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<DetalleMatricula>().HasOne(d => d.Curso).WithMany().HasForeignKey(d => d.CursoId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Horario>().HasOne(a => a.AsignacionDocente).WithMany(a => a.Horarios).HasForeignKey(h => h.AsignacionDocenteId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Docente>().HasIndex(d => d.Dni).IsUnique();
        }
    }
}
