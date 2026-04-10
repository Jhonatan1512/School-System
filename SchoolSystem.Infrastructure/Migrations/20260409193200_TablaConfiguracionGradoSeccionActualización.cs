using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TablaConfiguracionGradoSeccionActualización : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionGradoSecciones_PeriodoacademicoId",
                table: "ConfiguracionGradoSecciones",
                column: "PeriodoacademicoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConfiguracionGradoSecciones_PeriodoAcademicos_PeriodoacademicoId",
                table: "ConfiguracionGradoSecciones",
                column: "PeriodoacademicoId",
                principalTable: "PeriodoAcademicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConfiguracionGradoSecciones_PeriodoAcademicos_PeriodoacademicoId",
                table: "ConfiguracionGradoSecciones");

            migrationBuilder.DropIndex(
                name: "IX_ConfiguracionGradoSecciones_PeriodoacademicoId",
                table: "ConfiguracionGradoSecciones");
        }
    }
}
