using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyBasket.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCoulmnIsDisabledTOApplicationUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDisabled",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d98d583d-71cf-4a5b-8c97-0f875db2b473",
                columns: new[] { "IsDisabled", "PasswordHash" },
                values: new object[] { false, "AQAAAAIAAYagAAAAELHfaAPKvEZ5siyGegkrbY268ucupHWlS6TAkI919T5XdZbp0lPgrMq2yo7KiNASOg==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDisabled",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d98d583d-71cf-4a5b-8c97-0f875db2b473",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEElHUVVSff7E+TmhKgmPnBTglAVcAgTIErN1e5RHPnz+EYB3JXKxbNRAU+BszByReA==");
        }
    }
}
