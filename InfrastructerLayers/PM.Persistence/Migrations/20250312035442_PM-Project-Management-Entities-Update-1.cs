using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PM.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PMProjectManagementEntitiesUpdate1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Document_Mission_MissionId1",
                table: "Document");

            migrationBuilder.DropForeignKey(
                name: "FK_Document_Project_ProjectId1",
                table: "Document");

            migrationBuilder.DropForeignKey(
                name: "FK_Mission_Status_StatusId1",
                table: "Mission");

            migrationBuilder.DropForeignKey(
                name: "FK_Plan_Status_StatusId1",
                table: "Plan");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_Status_StatusId1",
                table: "Project");

            migrationBuilder.DropIndex(
                name: "IX_Project_StatusId1",
                table: "Project");

            migrationBuilder.DropIndex(
                name: "IX_Plan_StatusId1",
                table: "Plan");

            migrationBuilder.DropIndex(
                name: "IX_Mission_StatusId1",
                table: "Mission");

            migrationBuilder.DropIndex(
                name: "IX_Document_MissionId1",
                table: "Document");

            migrationBuilder.DropIndex(
                name: "IX_Document_ProjectId1",
                table: "Document");

            migrationBuilder.DropColumn(
                name: "StatusId1",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "StatusId1",
                table: "Plan");

            migrationBuilder.DropColumn(
                name: "StatusId1",
                table: "Mission");

            migrationBuilder.DropColumn(
                name: "MissionId1",
                table: "Document");

            migrationBuilder.DropColumn(
                name: "ProjectId1",
                table: "Document");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StatusId1",
                table: "Project",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusId1",
                table: "Plan",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusId1",
                table: "Mission",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MissionId1",
                table: "Document",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProjectId1",
                table: "Document",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Project_StatusId1",
                table: "Project",
                column: "StatusId1");

            migrationBuilder.CreateIndex(
                name: "IX_Plan_StatusId1",
                table: "Plan",
                column: "StatusId1");

            migrationBuilder.CreateIndex(
                name: "IX_Mission_StatusId1",
                table: "Mission",
                column: "StatusId1");

            migrationBuilder.CreateIndex(
                name: "IX_Document_MissionId1",
                table: "Document",
                column: "MissionId1");

            migrationBuilder.CreateIndex(
                name: "IX_Document_ProjectId1",
                table: "Document",
                column: "ProjectId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Document_Mission_MissionId1",
                table: "Document",
                column: "MissionId1",
                principalTable: "Mission",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Document_Project_ProjectId1",
                table: "Document",
                column: "ProjectId1",
                principalTable: "Project",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Mission_Status_StatusId1",
                table: "Mission",
                column: "StatusId1",
                principalTable: "Status",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Plan_Status_StatusId1",
                table: "Plan",
                column: "StatusId1",
                principalTable: "Status",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Project_Status_StatusId1",
                table: "Project",
                column: "StatusId1",
                principalTable: "Status",
                principalColumn: "Id");
        }
    }
}
