using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishStudy.Migrations
{
    /// <inheritdoc />
    public partial class English_Study_v30 : Migration
    {
/*
 创建了听力表，包括听力试题表，听力段表，听力题目表
 */

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "listening_paper",
                columns: table => new
                {
                    listening_paper_id = table.Column<int>(type: "int", nullable: false, comment: "听力试题id")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    papre_title = table.Column<string>(type: "varchar(100)", nullable: false, comment: "听力试题标题")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    audio = table.Column<string>(type: "varchar(200)", nullable: false, comment: "听力音频URL")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    delete_sign = table.Column<int>(type: "int", nullable: false, comment: "删除标记位")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_listening_paper", x => x.listening_paper_id);
                },
                comment: "听力试题表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "part",
                columns: table => new
                {
                    part_id = table.Column<int>(type: "int", nullable: false, comment: "听力小题部分")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    listening_paper_id = table.Column<int>(type: "int", nullable: false),
                    original_text = table.Column<string>(type: "text", nullable: false, comment: "听力原文")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    part_title = table.Column<string>(type: "varchar(100)", nullable: false, comment: "题目")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    delete_sign = table.Column<int>(type: "int", nullable: false, comment: "删除标记位")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_part", x => x.part_id);
                    table.ForeignKey(
                        name: "FK_part_listening_paper_listening_paper_id",
                        column: x => x.listening_paper_id,
                        principalTable: "listening_paper",
                        principalColumn: "listening_paper_id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "听力段部分")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "listening_question",
                columns: table => new
                {
                    question_id = table.Column<int>(type: "int", nullable: false, comment: "听力小题题号")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    part_id = table.Column<int>(type: "int", nullable: false, comment: "听力段号"),
                    options_a = table.Column<string>(type: "varchar(255)", nullable: false, comment: "A选项")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    options_b = table.Column<string>(type: "varchar(255)", nullable: false, comment: "B选项")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    options_c = table.Column<string>(type: "varchar(255)", nullable: false, comment: "C选项")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    options_d = table.Column<string>(type: "varchar(255)", nullable: false, comment: "D选项")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    title = table.Column<string>(type: "varchar(255)", nullable: false, comment: "题目内容")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    answer = table.Column<string>(type: "varchar(10)", nullable: false, comment: "答案")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    delete_sign = table.Column<int>(type: "int", nullable: false, comment: "删除标记位")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_listening_question", x => x.question_id);
                    table.ForeignKey(
                        name: "FK_listening_question_part_part_id",
                        column: x => x.part_id,
                        principalTable: "part",
                        principalColumn: "part_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_listening_question_part_id",
                table: "listening_question",
                column: "part_id");

            migrationBuilder.CreateIndex(
                name: "IX_part_listening_paper_id",
                table: "part",
                column: "listening_paper_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "listening_question");

            migrationBuilder.DropTable(
                name: "part");

            migrationBuilder.DropTable(
                name: "listening_paper");
        }
    }
}
