using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Test.Migrations
{
    /// <inheritdoc />
    public partial class AddCommunityUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommunityUsers",
                columns: table => new
                {
                    communityId = table.Column<Guid>(type: "uuid", nullable: false),
                    userId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityUsers", x => new { x.communityId, x.userId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommunityUsers");
        }
    }
}
