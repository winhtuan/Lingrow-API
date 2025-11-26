using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lingrow.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class Lingrow_v1_5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_student_cards_user_account_student_id",
                table: "student_cards");

            migrationBuilder.DropIndex(
                name: "IX_student_cards_student_id",
                table: "student_cards");

            migrationBuilder.DropColumn(
                name: "student_id",
                table: "student_cards");

            migrationBuilder.AddColumn<Guid>(
                name: "UserAccountUserId",
                table: "student_cards",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_student_cards_UserAccountUserId",
                table: "student_cards",
                column: "UserAccountUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_student_cards_user_account_UserAccountUserId",
                table: "student_cards",
                column: "UserAccountUserId",
                principalTable: "user_account",
                principalColumn: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_student_cards_user_account_UserAccountUserId",
                table: "student_cards");

            migrationBuilder.DropIndex(
                name: "IX_student_cards_UserAccountUserId",
                table: "student_cards");

            migrationBuilder.DropColumn(
                name: "UserAccountUserId",
                table: "student_cards");

            migrationBuilder.AddColumn<Guid>(
                name: "student_id",
                table: "student_cards",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_student_cards_student_id",
                table: "student_cards",
                column: "student_id");

            migrationBuilder.AddForeignKey(
                name: "FK_student_cards_user_account_student_id",
                table: "student_cards",
                column: "student_id",
                principalTable: "user_account",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
