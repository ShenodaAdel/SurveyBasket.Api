using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyBasket.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixVoteAnswerIdColumnType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VoteAnswers_Answers_AnswerId1",
                table: "VoteAnswers");

            migrationBuilder.DropIndex(
                name: "IX_VoteAnswers_AnswerId1",
                table: "VoteAnswers");

            migrationBuilder.DropColumn(
                name: "AnswerId1",
                table: "VoteAnswers");

            migrationBuilder.AlterColumn<int>(
                name: "AnswerId",
                table: "VoteAnswers",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_VoteAnswers_AnswerId",
                table: "VoteAnswers",
                column: "AnswerId");

            migrationBuilder.AddForeignKey(
                name: "FK_VoteAnswers_Answers_AnswerId",
                table: "VoteAnswers",
                column: "AnswerId",
                principalTable: "Answers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VoteAnswers_Answers_AnswerId",
                table: "VoteAnswers");

            migrationBuilder.DropIndex(
                name: "IX_VoteAnswers_AnswerId",
                table: "VoteAnswers");

            migrationBuilder.AlterColumn<string>(
                name: "AnswerId",
                table: "VoteAnswers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "AnswerId1",
                table: "VoteAnswers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_VoteAnswers_AnswerId1",
                table: "VoteAnswers",
                column: "AnswerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_VoteAnswers_Answers_AnswerId1",
                table: "VoteAnswers",
                column: "AnswerId1",
                principalTable: "Answers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
