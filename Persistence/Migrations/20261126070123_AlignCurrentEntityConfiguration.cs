using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AlignCurrentEntityConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Artists_Email",
                table: "Artists");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SubmittedAt",
                table: "TrackDistributions",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<byte>(
                name: "UserRole",
                table: "AspNetUsers",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)3,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<byte>(
                name: "Gender",
                table: "AspNetUsers",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Tracks_ISRC",
                table: "Tracks",
                sql: "LEN([ISRC]) = 12 AND [ISRC] NOT LIKE '%[^A-Z0-9]%' AND SUBSTRING([ISRC], 1, 2) NOT LIKE '%[^A-Z]%' AND SUBSTRING([ISRC], 6, 7) NOT LIKE '%[^0-9]%'");

            migrationBuilder.CreateIndex(
                name: "IX_Artists_Email",
                table: "Artists",
                column: "Email",
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Artists_Email",
                table: "Artists",
                sql: "[Email] LIKE '%_@_%._%'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Tracks_ISRC",
                table: "Tracks");

            migrationBuilder.DropIndex(
                name: "IX_Artists_Email",
                table: "Artists");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Artists_Email",
                table: "Artists");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SubmittedAt",
                table: "TrackDistributions",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "SYSUTCDATETIME()");

            migrationBuilder.AlterColumn<byte>(
                name: "UserRole",
                table: "AspNetUsers",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldDefaultValue: (byte)3);

            migrationBuilder.AlterColumn<byte>(
                name: "Gender",
                table: "AspNetUsers",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldDefaultValue: (byte)0);

            migrationBuilder.CreateIndex(
                name: "IX_Artists_Email",
                table: "Artists",
                column: "Email");
        }
    }
}
