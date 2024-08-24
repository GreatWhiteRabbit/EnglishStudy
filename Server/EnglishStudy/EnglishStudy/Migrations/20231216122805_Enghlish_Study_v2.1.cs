using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishStudy.Migrations
{
    /// <inheritdoc />
    public partial class Enghlish_Study_v21 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "everyday_english",
                columns: table => new
                {
                    everyday_id = table.Column<int>(type: "int", nullable: false, comment: "每日英语主键")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    title = table.Column<string>(type: "varchar(200)", nullable: false, comment: "标题")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    content = table.Column<string>(type: "text", nullable: false, comment: "内容")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    translations = table.Column<string>(type: "text", nullable: false, comment: "内容翻译")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    delete_sign = table.Column<int>(type: "int", nullable: false, comment: "删除标志位"),
                    time = table.Column<DateTime>(type: "timestamp", nullable: false, comment: "创建时间"),
                    title_translations = table.Column<string>(type: "varchar(200)", nullable: false, comment: "标题翻译")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_everyday_english", x => x.everyday_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "passage",
                columns: table => new
                {
                    passage_id = table.Column<int>(type: "int", nullable: false, comment: "文章ID")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    title = table.Column<string>(type: "varchar(100)", nullable: false, comment: "文章标题")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    content = table.Column<string>(type: "text", nullable: false, comment: "文章内容")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    delete_sign = table.Column<int>(type: "int", nullable: false, comment: "删除标记位")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_passage", x => x.passage_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "question",
                columns: table => new
                {
                    question_id = table.Column<int>(type: "int", nullable: false, comment: "问题ID")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PassageId = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "varchar(255)", nullable: false, comment: "具体问题")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    options_a = table.Column<string>(type: "varchar(255)", nullable: false, comment: "A选项")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    options_b = table.Column<string>(type: "varchar(255)", nullable: false, comment: "B选项")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    options_c = table.Column<string>(type: "varchar(255)", nullable: false, comment: "C选项")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    options_d = table.Column<string>(type: "varchar(255)", nullable: false, comment: "D选项")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    answer = table.Column<string>(type: "varchar(10)", nullable: false, comment: "正确答案")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    explanation = table.Column<string>(type: "text", nullable: false, comment: "答案解释")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    delete_sign = table.Column<int>(type: "int", nullable: false, comment: "删除标记位")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_question", x => x.question_id);
                    table.ForeignKey(
                        name: "FK_question_passage_PassageId",
                        column: x => x.PassageId,
                        principalTable: "passage",
                        principalColumn: "passage_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_question_PassageId",
                table: "question",
                column: "PassageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "everyday_english");

            migrationBuilder.DropTable(
                name: "question");

            migrationBuilder.DropTable(
                name: "passage");
        }
    }
}
