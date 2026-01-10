using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Remp.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class renameAvatar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AvataUrl",
                table: "Agents",
                newName: "AvatarUrl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AvatarUrl",
                table: "Agents",
                newName: "AvataUrl");
        }
    }
}
