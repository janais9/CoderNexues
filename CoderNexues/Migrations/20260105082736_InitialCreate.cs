using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoderNexues.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Camps",
                columns: table => new
                {
                    CampID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Camps", x => x.CampID);
                });

            migrationBuilder.CreateTable(
                name: "FeedbackCategories",
                columns: table => new
                {
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackCategories", x => x.CategoryID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    TaskID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampID = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaxScore = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.TaskID);
                    table.ForeignKey(
                        name: "FK_Tasks_Camps_CampID",
                        column: x => x.CampID,
                        principalTable: "Camps",
                        principalColumn: "CampID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CampUsers",
                columns: table => new
                {
                    CampUserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    RoleInCamp = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampUsers", x => x.CampUserID);
                    table.ForeignKey(
                        name: "FK_CampUsers_Camps_CampID",
                        column: x => x.CampID,
                        principalTable: "Camps",
                        principalColumn: "CampID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CampUsers_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentPerformances",
                columns: table => new
                {
                    PerformanceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentID = table.Column<int>(type: "int", nullable: false),
                    CampID = table.Column<int>(type: "int", nullable: false),
                    AvgScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Strengths = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Weaknesses = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentPerformances", x => x.PerformanceID);
                    table.ForeignKey(
                        name: "FK_StudentPerformances_Camps_CampID",
                        column: x => x.CampID,
                        principalTable: "Camps",
                        principalColumn: "CampID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentPerformances_Users_StudentID",
                        column: x => x.StudentID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Submissions",
                columns: table => new
                {
                    SubmissionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskID = table.Column<int>(type: "int", nullable: false),
                    StudentID = table.Column<int>(type: "int", nullable: false),
                    SubmissionLink = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Submissions", x => x.SubmissionID);
                    table.ForeignKey(
                        name: "FK_Submissions_Tasks_TaskID",
                        column: x => x.TaskID,
                        principalTable: "Tasks",
                        principalColumn: "TaskID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Submissions_Users_StudentID",
                        column: x => x.StudentID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Evaluations",
                columns: table => new
                {
                    EvaluationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubmissionID = table.Column<int>(type: "int", nullable: false),
                    EvaluatorID = table.Column<int>(type: "int", nullable: false),
                    CategoryID = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    Feedback = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EvaluatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evaluations", x => x.EvaluationID);
                    table.ForeignKey(
                        name: "FK_Evaluations_FeedbackCategories_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "FeedbackCategories",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Evaluations_Submissions_SubmissionID",
                        column: x => x.SubmissionID,
                        principalTable: "Submissions",
                        principalColumn: "SubmissionID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Evaluations_Users_EvaluatorID",
                        column: x => x.EvaluatorID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CampUsers_CampID",
                table: "CampUsers",
                column: "CampID");

            migrationBuilder.CreateIndex(
                name: "IX_CampUsers_UserID",
                table: "CampUsers",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluations_CategoryID",
                table: "Evaluations",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluations_EvaluatorID",
                table: "Evaluations",
                column: "EvaluatorID");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluations_SubmissionID",
                table: "Evaluations",
                column: "SubmissionID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentPerformances_CampID",
                table: "StudentPerformances",
                column: "CampID");

            migrationBuilder.CreateIndex(
                name: "IX_StudentPerformances_StudentID",
                table: "StudentPerformances",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_StudentID",
                table: "Submissions",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_TaskID",
                table: "Submissions",
                column: "TaskID");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CampID",
                table: "Tasks",
                column: "CampID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CampUsers");

            migrationBuilder.DropTable(
                name: "Evaluations");

            migrationBuilder.DropTable(
                name: "StudentPerformances");

            migrationBuilder.DropTable(
                name: "FeedbackCategories");

            migrationBuilder.DropTable(
                name: "Submissions");

            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Camps");
        }
    }
}
