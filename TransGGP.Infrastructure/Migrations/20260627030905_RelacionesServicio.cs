using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransGGP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RelacionesServicio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ConfiguracionId",
                table: "Servicios",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Servicios_ClienteId",
                table: "Servicios",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Servicios_ConfiguracionId",
                table: "Servicios",
                column: "ConfiguracionId");

            migrationBuilder.CreateIndex(
                name: "IX_Servicios_DollyId",
                table: "Servicios",
                column: "DollyId");

            migrationBuilder.CreateIndex(
                name: "IX_Servicios_OperadorId",
                table: "Servicios",
                column: "OperadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Servicios_SemirremolqueId",
                table: "Servicios",
                column: "SemirremolqueId");

            migrationBuilder.CreateIndex(
                name: "IX_Servicios_UnidadId",
                table: "Servicios",
                column: "UnidadId");

            migrationBuilder.AddForeignKey(
                name: "FK_Servicios_Clientes_ClienteId",
                table: "Servicios",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Servicios_Configuraciones_ConfiguracionId",
                table: "Servicios",
                column: "ConfiguracionId",
                principalTable: "Configuraciones",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Servicios_Dollys_DollyId",
                table: "Servicios",
                column: "DollyId",
                principalTable: "Dollys",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Servicios_Operadores_OperadorId",
                table: "Servicios",
                column: "OperadorId",
                principalTable: "Operadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Servicios_Semirremolques_SemirremolqueId",
                table: "Servicios",
                column: "SemirremolqueId",
                principalTable: "Semirremolques",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Servicios_Unidades_UnidadId",
                table: "Servicios",
                column: "UnidadId",
                principalTable: "Unidades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Servicios_Clientes_ClienteId",
                table: "Servicios");

            migrationBuilder.DropForeignKey(
                name: "FK_Servicios_Configuraciones_ConfiguracionId",
                table: "Servicios");

            migrationBuilder.DropForeignKey(
                name: "FK_Servicios_Dollys_DollyId",
                table: "Servicios");

            migrationBuilder.DropForeignKey(
                name: "FK_Servicios_Operadores_OperadorId",
                table: "Servicios");

            migrationBuilder.DropForeignKey(
                name: "FK_Servicios_Semirremolques_SemirremolqueId",
                table: "Servicios");

            migrationBuilder.DropForeignKey(
                name: "FK_Servicios_Unidades_UnidadId",
                table: "Servicios");

            migrationBuilder.DropIndex(
                name: "IX_Servicios_ClienteId",
                table: "Servicios");

            migrationBuilder.DropIndex(
                name: "IX_Servicios_ConfiguracionId",
                table: "Servicios");

            migrationBuilder.DropIndex(
                name: "IX_Servicios_DollyId",
                table: "Servicios");

            migrationBuilder.DropIndex(
                name: "IX_Servicios_OperadorId",
                table: "Servicios");

            migrationBuilder.DropIndex(
                name: "IX_Servicios_SemirremolqueId",
                table: "Servicios");

            migrationBuilder.DropIndex(
                name: "IX_Servicios_UnidadId",
                table: "Servicios");

            migrationBuilder.AlterColumn<int>(
                name: "ConfiguracionId",
                table: "Servicios",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
