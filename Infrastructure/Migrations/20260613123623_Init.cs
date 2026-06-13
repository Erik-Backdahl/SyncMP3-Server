using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DownloadedSongs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SongId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NetworkId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OriginId = table.Column<Guid>(type: "uniqueidentifier", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DownloadedSongs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Networks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Networks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Songs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NetworkId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DownloadedSongId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    DurationSeconds = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Songs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Songs_Networks_NetworkId",
                        column: x => x.NetworkId,
                        principalTable: "Networks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NetworkId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Premium = table.Column<bool>(type: "bit", nullable: false),
                    PremiumExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Networks_NetworkId",
                        column: x => x.NetworkId,
                        principalTable: "Networks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RequestedSongs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestedSongs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestedSongs_Songs_Id",
                        column: x => x.Id,
                        principalTable: "Songs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SongUser",
                columns: table => new
                {
                    CurrentSongsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DownloadedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SongUser", x => new { x.CurrentSongsId, x.DownloadedById });
                    table.ForeignKey(
                        name: "FK_SongUser_Songs_CurrentSongsId",
                        column: x => x.CurrentSongsId,
                        principalTable: "Songs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SongUser_Users_DownloadedById",
                        column: x => x.DownloadedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequestedSongUser",
                columns: table => new
                {
                    RequestedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestedSongsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestedSongUser", x => new { x.RequestedById, x.RequestedSongsId });
                    table.ForeignKey(
                        name: "FK_RequestedSongUser_RequestedSongs_RequestedSongsId",
                        column: x => x.RequestedSongsId,
                        principalTable: "RequestedSongs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequestedSongUser_Users_RequestedById",
                        column: x => x.RequestedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DownloadedSongs_NetworkId",
                table: "DownloadedSongs",
                column: "NetworkId");

            migrationBuilder.CreateIndex(
                name: "IX_DownloadedSongs_SongId",
                table: "DownloadedSongs",
                column: "SongId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Networks_OwnerId",
                table: "Networks",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestedSongUser_RequestedSongsId",
                table: "RequestedSongUser",
                column: "RequestedSongsId");

            migrationBuilder.CreateIndex(
                name: "IX_Songs_NetworkId",
                table: "Songs",
                column: "NetworkId");

            migrationBuilder.CreateIndex(
                name: "IX_SongUser_DownloadedById",
                table: "SongUser",
                column: "DownloadedById");

            migrationBuilder.CreateIndex(
                name: "IX_Users_NetworkId",
                table: "Users",
                column: "NetworkId");

            migrationBuilder.AddForeignKey(
                name: "FK_DownloadedSongs_Networks_NetworkId",
                table: "DownloadedSongs",
                column: "NetworkId",
                principalTable: "Networks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DownloadedSongs_Songs_SongId",
                table: "DownloadedSongs",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Networks_Users_OwnerId",
                table: "Networks",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Networks_NetworkId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "DownloadedSongs");

            migrationBuilder.DropTable(
                name: "RequestedSongUser");

            migrationBuilder.DropTable(
                name: "SongUser");

            migrationBuilder.DropTable(
                name: "RequestedSongs");

            migrationBuilder.DropTable(
                name: "Songs");

            migrationBuilder.DropTable(
                name: "Networks");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
