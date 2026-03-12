using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ActualizacionTablaAsignacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GradoId",
                table: "AsignacionDocentes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AsignacionDocentes_GradoId",
                table: "AsignacionDocentes",
                column: "GradoId");

            migrationBuilder.AddForeignKey(
                name: "FK_AsignacionDocentes_Grados_GradoId",
                table: "AsignacionDocentes",
                column: "GradoId",
                principalTable: "Grados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AsignacionDocentes_Grados_GradoId",
                table: "AsignacionDocentes");

            migrationBuilder.DropIndex(
                name: "IX_AsignacionDocentes_GradoId",
                table: "AsignacionDocentes");

            migrationBuilder.DropColumn(
                name: "GradoId",
                table: "AsignacionDocentes");
        }
    }
}
