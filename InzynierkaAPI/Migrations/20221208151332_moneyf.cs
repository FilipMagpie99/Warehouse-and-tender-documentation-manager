using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InzynierkaAPI.Migrations
{
    public partial class moneyf : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {


    migrationBuilder.CreateTable(
                name: "RaportMagazynu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ilosc = table.Column<int>(type: "int", nullable: false),
                    CenaJednostkowa = table.Column<decimal>(type: "decimal(10)", precision: 10, nullable: false),
                    TypOperacji = table.Column<int>(type: "int", nullable: false),
                    ProduktId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StrefaId = table.Column<int>(type: "int", nullable: false),
                    DostawcaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaportMagazynu", x => x.Id);

                    table.ForeignKey(
                        name: "FK_RaportMagazynu_Produkt_ProduktId",
                        column: x => x.ProduktId,
                        principalTable: "Produkt",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RaportMagazynu_Strefa_StrefaId",
                        column: x => x.StrefaId,
                        principalTable: "Strefa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RaportMagazynu_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            


            migrationBuilder.CreateIndex(
                name: "IX_RaportMagazynu_DostawcaId",
                table: "RaportMagazynu",
                column: "DostawcaId");

            migrationBuilder.CreateIndex(
                name: "IX_RaportMagazynu_ProduktId",
                table: "RaportMagazynu",
                column: "ProduktId");

            migrationBuilder.CreateIndex(
                name: "IX_RaportMagazynu_StrefaId",
                table: "RaportMagazynu",
                column: "StrefaId");

            migrationBuilder.CreateIndex(
                name: "IX_RaportMagazynu_UserId",
                table: "RaportMagazynu",
                column: "UserId");


        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
