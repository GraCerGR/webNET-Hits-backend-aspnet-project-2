using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Test.Migrations
{
    /// <inheritdoc />
    public partial class AddRenameCreateTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "createDate",
                table: "Communities",
                newName: "createTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "createTime",
                table: "Communities",
                newName: "createDate");
        }
    }
}
