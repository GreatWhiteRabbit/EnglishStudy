using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishStudy.Migrations
{
    /// <inheritdoc />
    public partial class Add_MessageTable_ReceiveTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "message",
                columns: table => new
                {
                    message_id = table.Column<int>(type: "int", nullable: false, comment: "主键id")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    title = table.Column<string>(type: "varchar(100)", nullable: false, comment: "消息标题")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    content = table.Column<string>(type: "varchar(400)", nullable: false, comment: "消息内容")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    time = table.Column<DateTime>(type: "timestamp", nullable: false, comment: "发布时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_message", x => x.message_id);
                },
                comment: "系统消息表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "receive",
                columns: table => new
                {
                    receive_id = table.Column<int>(type: "int", nullable: false, comment: "主键Id")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    message_id = table.Column<int>(type: "int", nullable: false, comment: "消息id"),
                    user_id = table.Column<int>(type: "int", nullable: false, comment: "用户id"),
                    read = table.Column<int>(type: "int", nullable: false, comment: "消息是否已读"),
                    time = table.Column<DateTime>(type: "timestamp", nullable: false, comment: "消息读取时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_receive", x => x.receive_id);
                },
                comment: "消息接收表")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "message");

            migrationBuilder.DropTable(
                name: "receive");
        }
    }
}
