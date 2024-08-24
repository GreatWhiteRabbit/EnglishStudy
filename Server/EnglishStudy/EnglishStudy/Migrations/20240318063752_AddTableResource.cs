using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishStudy.Migrations
{
    /// <inheritdoc />
    public partial class AddTableResource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "resource",
                columns: table => new
                {
                    ResourceId = table.Column<int>(type: "int", nullable: false, comment: "资源Id")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(200)", nullable: false, comment: "资源名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    url = table.Column<string>(type: "varchar(200)", nullable: false, comment: "资源URL")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    image = table.Column<string>(type: "varchar(200)", nullable: false, comment: "封面")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sum = table.Column<int>(type: "int", nullable: false, comment: "下载次数"),
                    time = table.Column<DateTime>(type: "timestamp", nullable: false, comment: "添加时间"),
                    delete_sign = table.Column<bool>(type: "tinyint(1)", nullable: false, comment: "是否显示")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resource", x => x.ResourceId);
                },
                comment: "资源")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "resource");
        }
    }
}
