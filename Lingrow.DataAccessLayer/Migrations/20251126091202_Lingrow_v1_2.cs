using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lingrow.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class Lingrow_v1_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "user_account",
                keyColumn: "user_id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "user_account",
                type: "character varying(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "age",
                table: "user_account",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "bio",
                table: "user_account",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "expertise",
                table: "user_account",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "goals",
                table: "user_account",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "hourly_rate",
                table: "user_account",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "languages",
                table: "user_account",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "level",
                table: "user_account",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "preferred_language",
                table: "user_account",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "student_cards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    tutor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    student_id = table.Column<Guid>(type: "uuid", nullable: false),
                    display_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    tags = table.Column<string>(type: "jsonb", nullable: true),
                    color = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_cards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_student_cards_user_account_student_id",
                        column: x => x.student_id,
                        principalTable: "user_account",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_student_cards_user_account_tutor_id",
                        column: x => x.tutor_id,
                        principalTable: "user_account",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "schedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    tutor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    student_card_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    start_time = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    end_time = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_schedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_schedules_student_cards_student_card_id",
                        column: x => x.student_card_id,
                        principalTable: "student_cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_schedules_user_account_tutor_id",
                        column: x => x.tutor_id,
                        principalTable: "user_account",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "user_account",
                columns: new[] { "user_id", "avatar_url", "cognito_sub", "created_at", "date_of_birth", "deleted_at", "Discriminator", "email", "email_confirmed", "email_confirmed_at", "full_name", "gender", "last_login_at", "role", "status", "updated_at", "username" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), "https://www.ibm.com/content/dam/adobe-cms/instana/media_logo/AWS-EC2.png/_jcr_content/renditions/cq5dam.web.1280.1280.png", "sub-0001", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateOnly(2004, 6, 21), null, "UserAccount", "winhtuan.dev@gmail.com", true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Nguyen Minh A", 'M', new DateTime(2025, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), "admin", "Active", null, "minha" });

            migrationBuilder.CreateIndex(
                name: "IX_schedules_student_card_id_start_time",
                table: "schedules",
                columns: new[] { "student_card_id", "start_time" });

            migrationBuilder.CreateIndex(
                name: "IX_schedules_tutor_id_start_time",
                table: "schedules",
                columns: new[] { "tutor_id", "start_time" });

            migrationBuilder.CreateIndex(
                name: "IX_student_cards_student_id",
                table: "student_cards",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "IX_student_cards_tutor_id",
                table: "student_cards",
                column: "tutor_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "schedules");

            migrationBuilder.DropTable(
                name: "student_cards");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "user_account");

            migrationBuilder.DropColumn(
                name: "age",
                table: "user_account");

            migrationBuilder.DropColumn(
                name: "bio",
                table: "user_account");

            migrationBuilder.DropColumn(
                name: "expertise",
                table: "user_account");

            migrationBuilder.DropColumn(
                name: "goals",
                table: "user_account");

            migrationBuilder.DropColumn(
                name: "hourly_rate",
                table: "user_account");

            migrationBuilder.DropColumn(
                name: "languages",
                table: "user_account");

            migrationBuilder.DropColumn(
                name: "level",
                table: "user_account");

            migrationBuilder.DropColumn(
                name: "preferred_language",
                table: "user_account");
        }
    }
}
