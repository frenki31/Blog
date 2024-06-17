using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeReal.Migrations
{
    /// <inheritdoc />
    public partial class Categories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_ApplicationUserId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Comments_ParentCommentIDBR_Comment",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Posts_PostIDBR_Post",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_AspNetUsers_ApplicationUserId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Files_DocumentIDBR_Document",
                table: "Posts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Posts",
                table: "Posts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pages",
                table: "Pages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Files",
                table: "Files");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comments",
                table: "Comments");

            migrationBuilder.RenameTable(
                name: "Posts",
                newName: "BR_Posts");

            migrationBuilder.RenameTable(
                name: "Pages",
                newName: "BR_Pages");

            migrationBuilder.RenameTable(
                name: "Files",
                newName: "BR_Files");

            migrationBuilder.RenameTable(
                name: "Comments",
                newName: "BR_Comments");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_DocumentIDBR_Document",
                table: "BR_Posts",
                newName: "IX_BR_Posts_DocumentIDBR_Document");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_ApplicationUserId",
                table: "BR_Posts",
                newName: "IX_BR_Posts_ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_PostIDBR_Post",
                table: "BR_Comments",
                newName: "IX_BR_Comments_PostIDBR_Post");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_ParentCommentIDBR_Comment",
                table: "BR_Comments",
                newName: "IX_BR_Comments_ParentCommentIDBR_Comment");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_ApplicationUserId",
                table: "BR_Comments",
                newName: "IX_BR_Comments_ApplicationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BR_Posts",
                table: "BR_Posts",
                column: "IDBR_Post");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BR_Pages",
                table: "BR_Pages",
                column: "IDBR_Page");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BR_Files",
                table: "BR_Files",
                column: "IDBR_Document");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BR_Comments",
                table: "BR_Comments",
                column: "IDBR_Comment");

            migrationBuilder.CreateTable(
                name: "BR_Categories",
                columns: table => new
                {
                    IDBR_Category = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentCategoryIDBR_Category = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BR_Categories", x => x.IDBR_Category);
                    table.ForeignKey(
                        name: "FK_BR_Categories_BR_Categories_ParentCategoryIDBR_Category",
                        column: x => x.ParentCategoryIDBR_Category,
                        principalTable: "BR_Categories",
                        principalColumn: "IDBR_Category");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BR_Categories_ParentCategoryIDBR_Category",
                table: "BR_Categories",
                column: "ParentCategoryIDBR_Category");

            migrationBuilder.AddForeignKey(
                name: "FK_BR_Comments_AspNetUsers_ApplicationUserId",
                table: "BR_Comments",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BR_Comments_BR_Comments_ParentCommentIDBR_Comment",
                table: "BR_Comments",
                column: "ParentCommentIDBR_Comment",
                principalTable: "BR_Comments",
                principalColumn: "IDBR_Comment");

            migrationBuilder.AddForeignKey(
                name: "FK_BR_Comments_BR_Posts_PostIDBR_Post",
                table: "BR_Comments",
                column: "PostIDBR_Post",
                principalTable: "BR_Posts",
                principalColumn: "IDBR_Post");

            migrationBuilder.AddForeignKey(
                name: "FK_BR_Posts_AspNetUsers_ApplicationUserId",
                table: "BR_Posts",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BR_Posts_BR_Files_DocumentIDBR_Document",
                table: "BR_Posts",
                column: "DocumentIDBR_Document",
                principalTable: "BR_Files",
                principalColumn: "IDBR_Document");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BR_Comments_AspNetUsers_ApplicationUserId",
                table: "BR_Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_BR_Comments_BR_Comments_ParentCommentIDBR_Comment",
                table: "BR_Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_BR_Comments_BR_Posts_PostIDBR_Post",
                table: "BR_Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_BR_Posts_AspNetUsers_ApplicationUserId",
                table: "BR_Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_BR_Posts_BR_Files_DocumentIDBR_Document",
                table: "BR_Posts");

            migrationBuilder.DropTable(
                name: "BR_Categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BR_Posts",
                table: "BR_Posts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BR_Pages",
                table: "BR_Pages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BR_Files",
                table: "BR_Files");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BR_Comments",
                table: "BR_Comments");

            migrationBuilder.RenameTable(
                name: "BR_Posts",
                newName: "Posts");

            migrationBuilder.RenameTable(
                name: "BR_Pages",
                newName: "Pages");

            migrationBuilder.RenameTable(
                name: "BR_Files",
                newName: "Files");

            migrationBuilder.RenameTable(
                name: "BR_Comments",
                newName: "Comments");

            migrationBuilder.RenameIndex(
                name: "IX_BR_Posts_DocumentIDBR_Document",
                table: "Posts",
                newName: "IX_Posts_DocumentIDBR_Document");

            migrationBuilder.RenameIndex(
                name: "IX_BR_Posts_ApplicationUserId",
                table: "Posts",
                newName: "IX_Posts_ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_BR_Comments_PostIDBR_Post",
                table: "Comments",
                newName: "IX_Comments_PostIDBR_Post");

            migrationBuilder.RenameIndex(
                name: "IX_BR_Comments_ParentCommentIDBR_Comment",
                table: "Comments",
                newName: "IX_Comments_ParentCommentIDBR_Comment");

            migrationBuilder.RenameIndex(
                name: "IX_BR_Comments_ApplicationUserId",
                table: "Comments",
                newName: "IX_Comments_ApplicationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Posts",
                table: "Posts",
                column: "IDBR_Post");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pages",
                table: "Pages",
                column: "IDBR_Page");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Files",
                table: "Files",
                column: "IDBR_Document");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comments",
                table: "Comments",
                column: "IDBR_Comment");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_ApplicationUserId",
                table: "Comments",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_ParentCommentIDBR_Comment",
                table: "Comments",
                column: "ParentCommentIDBR_Comment",
                principalTable: "Comments",
                principalColumn: "IDBR_Comment");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Posts_PostIDBR_Post",
                table: "Comments",
                column: "PostIDBR_Post",
                principalTable: "Posts",
                principalColumn: "IDBR_Post");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_AspNetUsers_ApplicationUserId",
                table: "Posts",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Files_DocumentIDBR_Document",
                table: "Posts",
                column: "DocumentIDBR_Document",
                principalTable: "Files",
                principalColumn: "IDBR_Document");
        }
    }
}
