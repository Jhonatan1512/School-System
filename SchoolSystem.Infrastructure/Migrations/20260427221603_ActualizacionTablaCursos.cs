using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ActualizacionTablaCursos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PeriodoAcademicoId",
                table: "PlanEstudios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-user-seed-id",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "92b5e9b1-b202-41bc-b58d-604e1f2c7cf2", "AQAAAAIAAYagAAAAELWV8eg0B2GN/i8GlPvN7EBCI+eA2qxgRbFQ1O8+nQMGACT4ZCyI647Scc6hD1WMUQ==", "4bd23b24-dc0b-4e32-978c-ee651415b755" });

            migrationBuilder.CreateIndex(
                name: "IX_PlanEstudios_PeriodoAcademicoId",
                table: "PlanEstudios",
                column: "PeriodoAcademicoId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlanEstudios_PeriodoAcademicos_PeriodoAcademicoId",
                table: "PlanEstudios",
                column: "PeriodoAcademicoId",
                principalTable: "PeriodoAcademicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlanEstudios_PeriodoAcademicos_PeriodoAcademicoId",
                table: "PlanEstudios");

            migrationBuilder.DropIndex(
                name: "IX_PlanEstudios_PeriodoAcademicoId",
                table: "PlanEstudios");

            migrationBuilder.DropColumn(
                name: "PeriodoAcademicoId",
                table: "PlanEstudios");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-user-seed-id",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2bab1d64-d625-4a40-8b33-a80a0d0e8bd6", "AQAAAAIAAYagAAAAEPuDqTTMo2oPVxmSwg+0L0acsBlkHgbrht0eptmMWh7KbjfQuTLWnkcnTTUTZgiC6Q==", "0f70bf8f-dc87-4156-a520-d19515ff8c15" });
        }
    }
}
