using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransGGP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProtegerHistorialServicios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Servicios_Clientes_ClienteId",
                table: "Servicios");

            migrationBuilder.DropForeignKey(
                name: "FK_Servicios_Operadores_OperadorId",
                table: "Servicios");

            migrationBuilder.DropForeignKey(
                name: "FK_Servicios_Unidades_UnidadId",
                table: "Servicios");

            migrationBuilder.AddForeignKey(
                name: "FK_Servicios_Clientes_ClienteId",
                table: "Servicios",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Servicios_Operadores_OperadorId",
                table: "Servicios",
                column: "OperadorId",
                principalTable: "Operadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Servicios_Unidades_UnidadId",
                table: "Servicios",
                column: "UnidadId",
                principalTable: "Unidades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Servicios_Clientes_ClienteId",
                table: "Servicios");

            migrationBuilder.DropForeignKey(
                name: "FK_Servicios_Operadores_OperadorId",
                table: "Servicios");

            migrationBuilder.DropForeignKey(
                name: "FK_Servicios_Unidades_UnidadId",
                table: "Servicios");

            migrationBuilder.AddForeignKey(
                name: "FK_Servicios_Clientes_ClienteId",
                table: "Servicios",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Servicios_Operadores_OperadorId",
                table: "Servicios",
                column: "OperadorId",
                principalTable: "Operadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Servicios_Unidades_UnidadId",
                table: "Servicios",
                column: "UnidadId",
                principalTable: "Unidades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
