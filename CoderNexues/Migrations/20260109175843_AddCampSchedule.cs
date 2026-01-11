using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoderNexues.Migrations
{
    /// <inheritdoc />
    public partial class AddCampSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CampSchedules",
                columns: table => new
                {
                    ScheduleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampID = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampSchedules", x => x.ScheduleID);
                    table.ForeignKey(
                        name: "FK_CampSchedules_Camps_CampID",
                        column: x => x.CampID,
                        principalTable: "Camps",
                        principalColumn: "CampID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CampSchedules_CampID",
                table: "CampSchedules",
                column: "CampID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CampSchedules");
        }
    }
}
