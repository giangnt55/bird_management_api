using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MainData.Migrations
{
    /// <inheritdoc />
    public partial class u2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Comments_TargetId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Post_TargetId",
                table: "Reports");

            migrationBuilder.RenameColumn(
                name: "TargetId",
                table: "Reports",
                newName: "PostId");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_TargetId",
                table: "Reports",
                newName: "IX_Reports_PostId");

            migrationBuilder.AddColumn<Guid>(
                name: "CommentId",
                table: "Reports",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Reports_CommentId",
                table: "Reports",
                column: "CommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Comments_CommentId",
                table: "Reports",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Post_PostId",
                table: "Reports",
                column: "PostId",
                principalTable: "Post",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Comments_CommentId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Post_PostId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_CommentId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "CommentId",
                table: "Reports");

            migrationBuilder.RenameColumn(
                name: "PostId",
                table: "Reports",
                newName: "TargetId");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_PostId",
                table: "Reports",
                newName: "IX_Reports_TargetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Comments_TargetId",
                table: "Reports",
                column: "TargetId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Post_TargetId",
                table: "Reports",
                column: "TargetId",
                principalTable: "Post",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
