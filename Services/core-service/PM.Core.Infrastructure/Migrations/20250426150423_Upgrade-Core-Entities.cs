using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PM.Core.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpgradeCoreEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProjectId",
                table: "Plans",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "MissionId",
                table: "MissionMembers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "ProjectMemberId",
                table: "MissionMembers",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "ProjectId",
                table: "Members",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PositionId",
                table: "Members",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Plans_ProjectId",
                table: "Plans",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_MissionMembers_MissionId",
                table: "MissionMembers",
                column: "MissionId");

            migrationBuilder.CreateIndex(
                name: "IX_MissionMembers_ProjectMemberId",
                table: "MissionMembers",
                column: "ProjectMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Members_PositionId",
                table: "Members",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Members_ProjectId",
                table: "Members",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Members_Positions_PositionId",
                table: "Members",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Members_Projects_ProjectId",
                table: "Members",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MissionMembers_Members_ProjectMemberId",
                table: "MissionMembers",
                column: "ProjectMemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MissionMembers_Missions_MissionId",
                table: "MissionMembers",
                column: "MissionId",
                principalTable: "Missions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Plans_Projects_ProjectId",
                table: "Plans",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Members_Positions_PositionId",
                table: "Members");

            migrationBuilder.DropForeignKey(
                name: "FK_Members_Projects_ProjectId",
                table: "Members");

            migrationBuilder.DropForeignKey(
                name: "FK_MissionMembers_Members_ProjectMemberId",
                table: "MissionMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_MissionMembers_Missions_MissionId",
                table: "MissionMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_Plans_Projects_ProjectId",
                table: "Plans");

            migrationBuilder.DropIndex(
                name: "IX_Plans_ProjectId",
                table: "Plans");

            migrationBuilder.DropIndex(
                name: "IX_MissionMembers_MissionId",
                table: "MissionMembers");

            migrationBuilder.DropIndex(
                name: "IX_MissionMembers_ProjectMemberId",
                table: "MissionMembers");

            migrationBuilder.DropIndex(
                name: "IX_Members_PositionId",
                table: "Members");

            migrationBuilder.DropIndex(
                name: "IX_Members_ProjectId",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Plans");

            migrationBuilder.DropColumn(
                name: "ProjectMemberId",
                table: "MissionMembers");

            migrationBuilder.AlterColumn<string>(
                name: "MissionId",
                table: "MissionMembers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ProjectId",
                table: "Members",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "PositionId",
                table: "Members",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
