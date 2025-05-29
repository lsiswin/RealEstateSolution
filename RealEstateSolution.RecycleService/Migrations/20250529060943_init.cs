using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateSolution.RecycleService.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecycleBins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    EntityData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: false),
                    DeleteTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RestoreTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsRestored = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecycleBins", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecycleBins");
        }
    }
}
