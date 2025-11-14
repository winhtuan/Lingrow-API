using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Lingrow.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class Lingrow_v1_0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_account",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    full_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    gender = table.Column<char>(type: "char(1)", nullable: false),
                    date_of_birth = table.Column<DateOnly>(type: "date", nullable: true),
                    avatar_url = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    updated_at = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_account", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "user_activity",
                columns: table => new
                {
                    activity_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    ref_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ref_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    metadata = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    ip = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    user_agent = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    correlation_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_activity", x => x.activity_id);
                    table.ForeignKey(
                        name: "FK_user_activity_user_account_user_id",
                        column: x => x.user_id,
                        principalTable: "user_account",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_login_data",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    role = table.Column<string>(type: "text", nullable: false),
                    password_salt = table.Column<byte[]>(type: "bytea", nullable: false),
                    password_hash = table.Column<byte[]>(type: "bytea", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    last_login_at = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    email_confirmed_at = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    access_failed_count = table.Column<int>(type: "integer", nullable: false),
                    lockout_end = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_login_data", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_user_login_data_user_account_user_id",
                        column: x => x.user_id,
                        principalTable: "user_account",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "user_account",
                columns: new[] { "user_id", "avatar_url", "created_at", "date_of_birth", "deleted_at", "full_name", "gender", "status", "updated_at" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "https://www.ibm.com/content/dam/adobe-cms/instana/media_logo/AWS-EC2.png/_jcr_content/renditions/cq5dam.web.1280.1280.png", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateOnly(2004, 6, 21), null, "Nguyen Minh A", 'M', "Active", null },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "https://tse3.mm.bing.net/th/id/OIP.JMspq1z3Vm2m00ioNzUtEgHaHa?cb=12&rs=1&pid=ImgDetMain&o=7&rm=3", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateOnly(2004, 6, 21), null, "Nguyen Minh B", 'M', "Active", null }
                });

            migrationBuilder.InsertData(
                table: "user_login_data",
                columns: new[] { "user_id", "access_failed_count", "created_at", "email", "email_confirmed", "email_confirmed_at", "last_login_at", "lockout_end", "password_hash", "password_salt", "role", "two_factor_enabled", "username" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "winhtuan.dev@gmail.com", false, null, null, null, new byte[] { 116, 63, 45, 102, 193, 171, 8, 42, 68, 233, 146, 96, 202, 43, 251, 179, 206, 71, 1, 251, 58, 48, 141, 11, 241, 199, 15, 85, 158, 160, 57, 116 }, new byte[] { 229, 111, 20, 109, 231, 252, 93, 204, 64, 123, 57, 235, 210, 227, 231, 88 }, "admin", false, "minha" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "winhtuan@gmail.com", false, null, null, null, new byte[] { 116, 63, 45, 102, 193, 171, 8, 42, 68, 233, 146, 96, 202, 43, 251, 179, 206, 71, 1, 251, 58, 48, 141, 11, 241, 199, 15, 85, 158, 160, 57, 116 }, new byte[] { 229, 111, 20, 109, 231, 252, 93, 204, 64, 123, 57, 235, 210, 227, 231, 88 }, "user", false, "minhb" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_activity_type",
                table: "user_activity",
                column: "type");

            migrationBuilder.CreateIndex(
                name: "IX_user_activity_user_id_created_at",
                table: "user_activity",
                columns: new[] { "user_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "IX_user_login_data_email",
                table: "user_login_data",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_login_data_username",
                table: "user_login_data",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_activity");

            migrationBuilder.DropTable(
                name: "user_login_data");

            migrationBuilder.DropTable(
                name: "user_account");
        }
    }
}
