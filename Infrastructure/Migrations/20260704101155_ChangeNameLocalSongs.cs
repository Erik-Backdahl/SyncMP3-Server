using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeNameLocalSongs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDownloadedSongs_Songs_DownloadedSongsId",
                table: "UserDownloadedSongs");

            migrationBuilder.RenameColumn(
                name: "DownloadedSongsId",
                table: "UserDownloadedSongs",
                newName: "LocalSongsId");

            migrationBuilder.RenameIndex(
                name: "IX_UserDownloadedSongs_DownloadedSongsId",
                table: "UserDownloadedSongs",
                newName: "IX_UserDownloadedSongs_LocalSongsId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDownloadedSongs_Songs_LocalSongsId",
                table: "UserDownloadedSongs",
                column: "LocalSongsId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDownloadedSongs_Songs_LocalSongsId",
                table: "UserDownloadedSongs");

            migrationBuilder.RenameColumn(
                name: "LocalSongsId",
                table: "UserDownloadedSongs",
                newName: "DownloadedSongsId");

            migrationBuilder.RenameIndex(
                name: "IX_UserDownloadedSongs_LocalSongsId",
                table: "UserDownloadedSongs",
                newName: "IX_UserDownloadedSongs_DownloadedSongsId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDownloadedSongs_Songs_DownloadedSongsId",
                table: "UserDownloadedSongs",
                column: "DownloadedSongsId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
