using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishStudy.Migrations
{
    /// <inheritdoc />
    public partial class Add_CommentTable_CommentReciveTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "comment",
                columns: table => new
                {
                    comment_id = table.Column<int>(type: "int", nullable: false, comment: "主键评论id")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    reply_comment_id = table.Column<int>(type: "int", nullable: false, comment: "被回复的一级评论id"),
                    user_id = table.Column<int>(type: "int", nullable: false, comment: "用户id"),
                    reply_user_id = table.Column<int>(type: "int", nullable: false, comment: "被回复的用户id"),
                    content = table.Column<string>(type: "varchar(500)", nullable: false, comment: "评论内容")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    time = table.Column<DateTime>(type: "timestamp", nullable: false, comment: "评论时间"),
                    delete_sign = table.Column<int>(type: "int", nullable: false, comment: "删除标记位")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comment", x => x.comment_id);
                },
                comment: "评论表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "comment_recive",
                columns: table => new
                {
                    comment_recive_id = table.Column<int>(type: "int", nullable: false, comment: "主键，评论回复接收id")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    comment_id = table.Column<int>(type: "int", nullable: false, comment: "评论的id"),
                    user_id = table.Column<int>(type: "int", nullable: false, comment: "用户id"),
                    read = table.Column<int>(type: "int", nullable: false, comment: "是否已读"),
                    time = table.Column<DateTime>(type: "timestamp", nullable: false, comment: "读取时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comment_recive", x => x.comment_recive_id);
                },
                comment: "评论接收表")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comment");

            migrationBuilder.DropTable(
                name: "comment_recive");
        }
    }
}
