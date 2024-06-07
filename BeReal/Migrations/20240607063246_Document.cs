using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeReal.Migrations
{
    /// <inheritdoc />
    public partial class Document : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Document",
                table: "Posts");

            migrationBuilder.AddColumn<int>(
                name: "DocumentId",
                table: "Posts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Data = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_DocumentId",
                table: "Posts",
                column: "DocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Files_DocumentId",
                table: "Posts",
                column: "DocumentId",
                principalTable: "Files",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Files_DocumentId",
                table: "Posts");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Posts_DocumentId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "Posts");

            migrationBuilder.AddColumn<string>(
                name: "Document",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
