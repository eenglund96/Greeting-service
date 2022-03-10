using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GreetingService.Infrastructure.Migrations
{
    public partial class AddedUserColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Greetings_invoices_InvoiceId",
                table: "Greetings");

            migrationBuilder.DropForeignKey(
                name: "FK_invoices_Users_SenderEmail",
                table: "invoices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_invoices",
                table: "invoices");

            migrationBuilder.RenameTable(
                name: "invoices",
                newName: "Invoices");

            migrationBuilder.RenameIndex(
                name: "IX_invoices_SenderEmail",
                table: "Invoices",
                newName: "IX_Invoices_SenderEmail");

            migrationBuilder.AddColumn<string>(
                name: "ApprovalCode",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovalExpiry",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ApprovalStatus",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ApprovalStatusNote",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Invoices",
                table: "Invoices",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Greetings_Invoices_InvoiceId",
                table: "Greetings",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Users_SenderEmail",
                table: "Invoices",
                column: "SenderEmail",
                principalTable: "Users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Greetings_Invoices_InvoiceId",
                table: "Greetings");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Users_SenderEmail",
                table: "Invoices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Invoices",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "ApprovalCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ApprovalExpiry",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ApprovalStatusNote",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Invoices",
                newName: "invoices");

            migrationBuilder.RenameIndex(
                name: "IX_Invoices_SenderEmail",
                table: "invoices",
                newName: "IX_invoices_SenderEmail");

            migrationBuilder.AddPrimaryKey(
                name: "PK_invoices",
                table: "invoices",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Greetings_invoices_InvoiceId",
                table: "Greetings",
                column: "InvoiceId",
                principalTable: "invoices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_invoices_Users_SenderEmail",
                table: "invoices",
                column: "SenderEmail",
                principalTable: "Users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
