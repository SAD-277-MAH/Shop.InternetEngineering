using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Shop.Data.Migrations
{
    public partial class MigSetting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    ShopName = table.Column<string>(maxLength: 100, nullable: true),
                    ShopDesc = table.Column<string>(nullable: true),
                    ShopKeyWords = table.Column<string>(nullable: true),
                    SmsApi = table.Column<string>(nullable: true),
                    SmsSender = table.Column<string>(maxLength: 15, nullable: true),
                    EmailAddress = table.Column<string>(maxLength: 100, nullable: true),
                    EmailPassword = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings");
        }
    }
}
