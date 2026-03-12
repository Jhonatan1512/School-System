using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTableTrimestre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Trimestre",
                table: "Calificaciones",
                newName: "TrimestreId");

            migrationBuilder.CreateTable(
                name: "Trimestres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstadoActivo = table.Column<bool>(type: "bit", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaCierre = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodoAcademicoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trimestres", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trimestres_PeriodoAcademicos_PeriodoAcademicoId",
                        column: x => x.PeriodoAcademicoId,
                        principalTable: "PeriodoAcademicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Calificaciones_TrimestreId",
                table: "Calificaciones",
                column: "TrimestreId");

            migrationBuilder.CreateIndex(
                name: "IX_Trimestres_PeriodoAcademicoId",
                table: "Trimestres",
                column: "PeriodoAcademicoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Calificaciones_Trimestres_TrimestreId",
                table: "Calificaciones",
                column: "TrimestreId",
                principalTable: "Trimestres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Calificaciones_Trimestres_TrimestreId",
                table: "Calificaciones");

            migrationBuilder.DropTable(
                name: "Trimestres");

            migrationBuilder.DropIndex(
                name: "IX_Calificaciones_TrimestreId",
                table: "Calificaciones");

            migrationBuilder.RenameColumn(
                name: "TrimestreId",
                table: "Calificaciones",
                newName: "Trimestre");
        }
    }
}
