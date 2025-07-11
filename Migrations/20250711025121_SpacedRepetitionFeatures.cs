using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuickMind.Migrations
{
    /// <inheritdoc />
    public partial class SpacedRepetitionFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Repetitions",
                table: "FlashCards",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "Interval",
                table: "FlashCards",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<double>(
                name: "EaseFactor",
                table: "FlashCards",
                type: "REAL",
                nullable: false,
                defaultValue: 2.5,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AddColumn<int>(
                name: "EasyInterval",
                table: "FlashCards",
                type: "INTEGER",
                nullable: false,
                defaultValue: 4);

            migrationBuilder.AddColumn<int>(
                name: "FuzzFactor",
                table: "FlashCards",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "GraduatingInterval",
                table: "FlashCards",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<bool>(
                name: "IsLeech",
                table: "FlashCards",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSuspended",
                table: "FlashCards",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Lapses",
                table: "FlashCards",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LearningDueDate",
                table: "FlashCards",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LearningStep",
                table: "FlashCards",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "FlashCards",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_FlashCards_DueDate",
                table: "FlashCards",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_FlashCards_Status",
                table: "FlashCards",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_FlashCards_Status_DueDate",
                table: "FlashCards",
                columns: new[] { "Status", "DueDate" });

            migrationBuilder.CreateIndex(
                name: "IX_FlashCards_Topic",
                table: "FlashCards",
                column: "Topic");

            migrationBuilder.CreateIndex(
                name: "IX_FlashCards_Type",
                table: "FlashCards",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FlashCards_DueDate",
                table: "FlashCards");

            migrationBuilder.DropIndex(
                name: "IX_FlashCards_Status",
                table: "FlashCards");

            migrationBuilder.DropIndex(
                name: "IX_FlashCards_Status_DueDate",
                table: "FlashCards");

            migrationBuilder.DropIndex(
                name: "IX_FlashCards_Topic",
                table: "FlashCards");

            migrationBuilder.DropIndex(
                name: "IX_FlashCards_Type",
                table: "FlashCards");

            migrationBuilder.DropColumn(
                name: "EasyInterval",
                table: "FlashCards");

            migrationBuilder.DropColumn(
                name: "FuzzFactor",
                table: "FlashCards");

            migrationBuilder.DropColumn(
                name: "GraduatingInterval",
                table: "FlashCards");

            migrationBuilder.DropColumn(
                name: "IsLeech",
                table: "FlashCards");

            migrationBuilder.DropColumn(
                name: "IsSuspended",
                table: "FlashCards");

            migrationBuilder.DropColumn(
                name: "Lapses",
                table: "FlashCards");

            migrationBuilder.DropColumn(
                name: "LearningDueDate",
                table: "FlashCards");

            migrationBuilder.DropColumn(
                name: "LearningStep",
                table: "FlashCards");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "FlashCards");

            migrationBuilder.AlterColumn<int>(
                name: "Repetitions",
                table: "FlashCards",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Interval",
                table: "FlashCards",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<double>(
                name: "EaseFactor",
                table: "FlashCards",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL",
                oldDefaultValue: 2.5);
        }
    }
}
