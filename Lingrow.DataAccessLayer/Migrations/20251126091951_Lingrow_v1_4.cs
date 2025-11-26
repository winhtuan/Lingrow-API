using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lingrow.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class Lingrow_v1_4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "hourly_rate",
                table: "user_account");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "hourly_rate",
                table: "user_account",
                type: "numeric",
                nullable: true);
        }
    }
}
