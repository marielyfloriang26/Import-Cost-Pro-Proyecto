using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capa_de_Datos.Migrations
{
    /// <inheritdoc />
    public partial class AddFechaCierreToOrdenImportacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCierre",
                table: "OrdenesImportacion",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaCierre",
                table: "OrdenesImportacion");
        }
    }
}
