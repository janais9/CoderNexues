using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoderNexues.Migrations
{
    /// <inheritdoc />
    public partial class AddCertificates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CertificateRequirements",
                columns: table => new
                {
                    ReqID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampID = table.Column<int>(type: "int", nullable: false),
                    CertificateName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deadline = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PassingScore = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateRequirements", x => x.ReqID);
                    table.ForeignKey(
                        name: "FK_CertificateRequirements_Camps_CampID",
                        column: x => x.CampID,
                        principalTable: "Camps",
                        principalColumn: "CampID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentCertificates",
                columns: table => new
                {
                    CertID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReqID = table.Column<int>(type: "int", nullable: false),
                    StudentID = table.Column<int>(type: "int", nullable: false),
                    HasTakenExam = table.Column<bool>(type: "bit", nullable: false),
                    ExamDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Score = table.Column<int>(type: "int", nullable: true),
                    IsPassed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentCertificates", x => x.CertID);
                    table.ForeignKey(
                        name: "FK_StudentCertificates_CertificateRequirements_ReqID",
                        column: x => x.ReqID,
                        principalTable: "CertificateRequirements",
                        principalColumn: "ReqID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentCertificates_Users_StudentID",
                        column: x => x.StudentID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CertificateRequirements_CampID",
                table: "CertificateRequirements",
                column: "CampID");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCertificates_ReqID",
                table: "StudentCertificates",
                column: "ReqID");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCertificates_StudentID",
                table: "StudentCertificates",
                column: "StudentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentCertificates");

            migrationBuilder.DropTable(
                name: "CertificateRequirements");
        }
    }
}
