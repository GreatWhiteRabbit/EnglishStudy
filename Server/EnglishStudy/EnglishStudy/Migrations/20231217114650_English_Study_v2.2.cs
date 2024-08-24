using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishStudy.Migrations
{
    /// <inheritdoc />
    public partial class English_Study_v22 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "en_record",
                columns: table => new
                {
                    record_id = table.Column<int>(type: "int", nullable: false, comment: "每日阅读记录id")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<int>(type: "int", nullable: false, comment: "用户id"),
                    everyday_id = table.Column<int>(type: "int", nullable: false, comment: "每日英语id"),
                    time = table.Column<DateTime>(type: "timestamp", nullable: false, comment: "阅读时间"),
                    delete_sign = table.Column<int>(type: "int", nullable: false, comment: "删除标记位")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_en_record", x => x.record_id);
                },
                comment: "每日英语阅读记录")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "en_record");
        }
    }
}
