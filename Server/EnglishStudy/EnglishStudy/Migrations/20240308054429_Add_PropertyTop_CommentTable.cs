using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishStudy.Migrations
{
    /// <inheritdoc />
    public partial class Add_PropertyTop_CommentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "top",
                table: "comment",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false,
                comment: "是否置顶");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "top",
                table: "comment");
        }
    }
}
