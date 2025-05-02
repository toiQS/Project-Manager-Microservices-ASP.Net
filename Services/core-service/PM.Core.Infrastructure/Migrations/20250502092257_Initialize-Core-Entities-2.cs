using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PM.Core.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitializeCoreEntities2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PlanId",
                table: "Missions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProjectMemberId",
                table: "Missions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Missions_PlanId",
                table: "Missions",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Missions_ProjectMemberId",
                table: "Missions",
                column: "ProjectMemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_Missions_Members_ProjectMemberId",
                table: "Missions",
                column: "ProjectMemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Missions_Plans_PlanId",
                table: "Missions",
                column: "PlanId",
                principalTable: "Plans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Missions_Members_ProjectMemberId",
                table: "Missions");

            migrationBuilder.DropForeignKey(
                name: "FK_Missions_Plans_PlanId",
                table: "Missions");

            migrationBuilder.DropIndex(
                name: "IX_Missions_PlanId",
                table: "Missions");

            migrationBuilder.DropIndex(
                name: "IX_Missions_ProjectMemberId",
                table: "Missions");

            migrationBuilder.DropColumn(
                name: "PlanId",
                table: "Missions");

            migrationBuilder.DropColumn(
                name: "ProjectMemberId",
                table: "Missions");
        }
    }
}
