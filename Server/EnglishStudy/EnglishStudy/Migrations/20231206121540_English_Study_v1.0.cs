using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishStudy.Migrations
{
    /// <inheritdoc />
    public partial class English_Study_v10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "words",
                columns: table => new
                {
                    word_id = table.Column<int>(type: "int", nullable: false, comment: "主键id")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    word = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "英语单词")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    phonetic = table.Column<string>(type: "varchar(70)", maxLength: 70, nullable: false, comment: "音标")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    paraphrase = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false, comment: "中文释义")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    type = table.Column<int>(type: "int", nullable: false, comment: "词汇类型(四六级)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_words", x => x.word_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "words");
        }
    }
}
