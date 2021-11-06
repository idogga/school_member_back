using Microsoft.EntityFrameworkCore.Migrations;

namespace School.Member.Api.Migrations
{
    public partial class PupilDto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teachers_Users_UserId",
                table: "Teachers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Teachers",
                table: "Teachers");

            migrationBuilder.RenameTable(
                name: "Teachers",
                newName: "Pupils");

            migrationBuilder.RenameIndex(
                name: "IX_Teachers_UserId",
                table: "Pupils",
                newName: "IX_Pupils_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pupils",
                table: "Pupils",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Pupils_Users_UserId",
                table: "Pupils",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pupils_Users_UserId",
                table: "Pupils");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pupils",
                table: "Pupils");

            migrationBuilder.RenameTable(
                name: "Pupils",
                newName: "Teachers");

            migrationBuilder.RenameIndex(
                name: "IX_Pupils_UserId",
                table: "Teachers",
                newName: "IX_Teachers_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Teachers",
                table: "Teachers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Teachers_Users_UserId",
                table: "Teachers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
