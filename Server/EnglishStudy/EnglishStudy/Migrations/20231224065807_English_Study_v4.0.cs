using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishStudy.Migrations
{
    /// <inheritdoc />
    public partial class English_Study_v40 : Migration
    {
        /// <inheritdoc />
        /*
         创建了两个新的表：
        wordPosition：用户记忆的单词位置
        wordRecord：用户记忆单词详细记录
         */
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "word_position",
                columns: table => new
                {
                    position_id = table.Column<int>(type: "int", nullable: false, comment: "主键")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<int>(type: "int", nullable: false, comment: "用户id"),
                    lastword_id = table.Column<int>(type: "int", nullable: false, comment: "上一次最后记忆的单词id"),
                    type = table.Column<int>(type: "int", nullable: false, comment: "单词类型"),
                    delete_sign = table.Column<int>(type: "int", nullable: false, comment: "删除标记位")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_word_position", x => x.position_id);
                },
                comment: "单词记忆位置")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "word_record",
                columns: table => new
                {
                    record_id = table.Column<int>(type: "int", nullable: false, comment: "记录id")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<int>(type: "int", nullable: false, comment: "用户id"),
                    word_type = table.Column<int>(type: "int", nullable: false, comment: "单词类型"),
                    detail = table.Column<string>(type: "text", nullable: false, comment: "详细记录")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    time = table.Column<DateTime>(type: "timestamp", nullable: false, comment: "创建时间"),
                    total = table.Column<int>(type: "int", nullable: false, comment: "记忆单词总数"),
                    delete_sign = table.Column<int>(type: "int", nullable: false, comment: "删除标记位")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_word_record", x => x.record_id);
                },
                comment: "记忆单词详细记录")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "word_position");

            migrationBuilder.DropTable(
                name: "word_record");
        }
    }
}
