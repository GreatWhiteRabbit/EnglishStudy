using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishStudy.Migrations
{
    /// <inheritdoc />
    public partial class English_Study_v31 : Migration
    {
        /// <inheritdoc />
       /*
        新添加了一个新的表听力做题记录，表结构和阅读理解做题记录的表
       结构是一样的，所以相关的代码编写可以根据阅读理解做题记录表来改写
       就行了
        */
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "listeningpaper_record",
                columns: table => new
                {
                    record_id = table.Column<int>(type: "int", nullable: false, comment: "记录id")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<int>(type: "int", nullable: false, comment: "用户id"),
                    listeningpaper_id = table.Column<int>(type: "int", nullable: false, comment: "听力试题id"),
                    detail = table.Column<string>(type: "text", nullable: false, comment: "做题详细记录")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    time = table.Column<DateTime>(type: "timestamp", nullable: false, comment: "创建时间"),
                    delete_sign = table.Column<int>(type: "int", nullable: false, comment: "删除标记位"),
                    accuracy = table.Column<double>(type: "double", nullable: false, comment: "准确度")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_listeningpaper_record", x => x.record_id);
                },
                comment: "听力试题记录")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "listeningpaper_record");
        }
    }
}
