using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeReal.Migrations
{
    /// <inheritdoc />
    public partial class Image : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "BR_Posts");

            migrationBuilder.AddColumn<int>(
                name: "ImageIDBR_Document",
                table: "BR_Posts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BR_Posts_ImageIDBR_Document",
                table: "BR_Posts",
                column: "ImageIDBR_Document");

            migrationBuilder.AddForeignKey(
                name: "FK_BR_Posts_BR_Files_ImageIDBR_Document",
                table: "BR_Posts",
                column: "ImageIDBR_Document",
                principalTable: "BR_Files",
                principalColumn: "IDBR_Document");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BR_Posts_BR_Files_ImageIDBR_Document",
                table: "BR_Posts");

            migrationBuilder.DropIndex(
                name: "IX_BR_Posts_ImageIDBR_Document",
                table: "BR_Posts");

            migrationBuilder.DropColumn(
                name: "ImageIDBR_Document",
                table: "BR_Posts");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "BR_Posts",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
