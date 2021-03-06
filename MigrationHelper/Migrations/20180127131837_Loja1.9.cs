﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace BaseGeral.Migrations
{
    public partial class Loja19 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsNFCe",
                table: "NotasFiscais",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsNFCe",
                table: "Inutilizacoes",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsNFCe",
                table: "NotasFiscais");

            migrationBuilder.DropColumn(
                name: "IsNFCe",
                table: "Inutilizacoes");
        }
    }
}
