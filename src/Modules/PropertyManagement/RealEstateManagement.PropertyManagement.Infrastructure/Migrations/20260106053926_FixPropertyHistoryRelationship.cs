using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstate.Property.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixPropertyHistoryRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerName",
                schema: "property",
                table: "Properties");

            migrationBuilder.AlterColumn<string>(
                name: "Currency",
                schema: "property",
                table: "Properties",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);

            migrationBuilder.AddColumn<int>(
                name: "Bathrooms",
                schema: "property",
                table: "Properties",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Bedrooms",
                schema: "property",
                table: "Properties",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "BranchId",
                schema: "property",
                table: "Properties",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "SizeSqMeters",
                schema: "property",
                table: "Properties",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "YearBuilt",
                schema: "property",
                table: "Properties",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PropertyHistories",
                schema: "property",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChangeType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OldValue = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    NewValue = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    ChangedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PropertyId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyHistories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PropertyHistories_PropertyId",
                schema: "property",
                table: "PropertyHistories",
                column: "PropertyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PropertyHistories",
                schema: "property");

            migrationBuilder.DropColumn(
                name: "Bathrooms",
                schema: "property",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "Bedrooms",
                schema: "property",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "BranchId",
                schema: "property",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "SizeSqMeters",
                schema: "property",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "YearBuilt",
                schema: "property",
                table: "Properties");

            migrationBuilder.AlterColumn<string>(
                name: "Currency",
                schema: "property",
                table: "Properties",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AddColumn<string>(
                name: "OwnerName",
                schema: "property",
                table: "Properties",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
