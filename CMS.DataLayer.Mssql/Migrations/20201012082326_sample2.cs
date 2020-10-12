using Microsoft.EntityFrameworkCore.Migrations;

namespace CMS.DataLayer.Mssql.Migrations
{
    public partial class sample2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "Status",
                table: "Sample",
                nullable: false,
                defaultValue: (byte)1,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<byte>(
                name: "Status",
                table: "AppUsers",
                nullable: false,
                defaultValue: (byte)1,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<byte>(
                name: "Status",
                table: "AppRoles",
                nullable: false,
                defaultValue: (byte)1,
                oldClrType: typeof(byte),
                oldType: "tinyint");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "Status",
                table: "Sample",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte),
                oldDefaultValue: (byte)1);

            migrationBuilder.AlterColumn<byte>(
                name: "Status",
                table: "AppUsers",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte),
                oldDefaultValue: (byte)1);

            migrationBuilder.AlterColumn<byte>(
                name: "Status",
                table: "AppRoles",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte),
                oldDefaultValue: (byte)1);
        }
    }
}
