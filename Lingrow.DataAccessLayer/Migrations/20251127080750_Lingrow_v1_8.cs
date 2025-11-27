using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lingrow.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class Lingrow_v1_8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_pinned",
                table: "schedules",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_pinned",
                table: "schedules");
        }
    }
}
