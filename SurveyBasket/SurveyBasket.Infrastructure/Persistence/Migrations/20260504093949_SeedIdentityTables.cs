using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SurveyBasket.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedIdentityTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "IsDefault", "IsDeleted", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "89c59864-cb81-4b1b-b34b-7e2623de448a", "6d8ccb85-ce2b-4439-baf2-52066ee1b0a7", true, false, "User", "USER" },
                    { "de21f544-014a-4e54-8faa-fff937973eb1", "290db1bb-60bc-4322-ae2f-fb93402828f7", false, false, "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "d98d583d-71cf-4a5b-8c97-0f875db2b473", 0, "44217654-d24f-4b87-9add-035b3dc227f1", "admin@surveybasket.com", true, "Survey Basket", "Admin", false, null, "ADMIN@SURVEYBASKET.COM", "ADMIN@SURVEYBASKET.COM", "AQAAAAIAAYagAAAAEElHUVVSff7E+TmhKgmPnBTglAVcAgTIErN1e5RHPnz+EYB3JXKxbNRAU+BszByReA==", null, false, "2B6427AF99F74F9EAA4959C471FAAD04", false, "admin@surveybasket.com" });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 1, "permissions", "polls:get", "de21f544-014a-4e54-8faa-fff937973eb1" },
                    { 2, "permissions", "polls:add", "de21f544-014a-4e54-8faa-fff937973eb1" },
                    { 3, "permissions", "polls:update", "de21f544-014a-4e54-8faa-fff937973eb1" },
                    { 4, "permissions", "polls:delete", "de21f544-014a-4e54-8faa-fff937973eb1" },
                    { 5, "permissions", "questions:get", "de21f544-014a-4e54-8faa-fff937973eb1" },
                    { 6, "permissions", "questions:add", "de21f544-014a-4e54-8faa-fff937973eb1" },
                    { 7, "permissions", "questions:update", "de21f544-014a-4e54-8faa-fff937973eb1" },
                    { 8, "permissions", "users:get", "de21f544-014a-4e54-8faa-fff937973eb1" },
                    { 9, "permissions", "users:add", "de21f544-014a-4e54-8faa-fff937973eb1" },
                    { 10, "permissions", "users:update", "de21f544-014a-4e54-8faa-fff937973eb1" },
                    { 11, "permissions", "roles:get", "de21f544-014a-4e54-8faa-fff937973eb1" },
                    { 12, "permissions", "roles:add", "de21f544-014a-4e54-8faa-fff937973eb1" },
                    { 13, "permissions", "roles:update", "de21f544-014a-4e54-8faa-fff937973eb1" },
                    { 14, "permissions", "results:get", "de21f544-014a-4e54-8faa-fff937973eb1" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "de21f544-014a-4e54-8faa-fff937973eb1", "d98d583d-71cf-4a5b-8c97-0f875db2b473" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "89c59864-cb81-4b1b-b34b-7e2623de448a");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "de21f544-014a-4e54-8faa-fff937973eb1", "d98d583d-71cf-4a5b-8c97-0f875db2b473" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "de21f544-014a-4e54-8faa-fff937973eb1");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d98d583d-71cf-4a5b-8c97-0f875db2b473");
        }
    }
}
