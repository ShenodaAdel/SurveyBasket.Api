using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyBasket.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditFieldsAndFixPollColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Ispublished",
                table: "Polls",
                newName: "IsPublished");

            migrationBuilder.RenameColumn(
                name: "StarstAt",
                table: "Polls",
                newName: "StartsAt");

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Polls",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "Polls",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Polls_CreatedById",
                table: "Polls",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Polls_UpdatedById",
                table: "Polls",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Polls_AspNetUsers_CreatedById",
                table: "Polls",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Polls_AspNetUsers_UpdatedById",
                table: "Polls",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Polls_AspNetUsers_CreatedById",
                table: "Polls");

            migrationBuilder.DropForeignKey(
                name: "FK_Polls_AspNetUsers_UpdatedById",
                table: "Polls");

            migrationBuilder.DropIndex(
                name: "IX_Polls_CreatedById",
                table: "Polls");

            migrationBuilder.DropIndex(
                name: "IX_Polls_UpdatedById",
                table: "Polls");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Polls");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "Polls");

            migrationBuilder.RenameColumn(
                name: "IsPublished",
                table: "Polls",
                newName: "Ispublished");

            migrationBuilder.RenameColumn(
                name: "StartsAt",
                table: "Polls",
                newName: "StarstAt");
        }
    }
}
