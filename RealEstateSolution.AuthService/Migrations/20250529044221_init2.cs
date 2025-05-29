using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RealEstateSolution.AuthService.Migrations
{
    /// <inheritdoc />
    public partial class init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "CreateTime", "Description", "Name", "NormalizedName", "UpdateTime" },
                values: new object[,]
                {
                    { "7924435e-c36a-4fdc-8532-90c73575e265", "76014c9a-7607-4bc3-8a21-c3d46d4dffa1", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "broker", "BROKER", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { "a311a857-78ec-44a6-836e-f1265ed18133", "81eac8c8-5b31-4067-a269-bb22d86a02e9", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "user", "USER", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { "b3a87c45-6456-4729-9e2d-d89bcf998322", "da32ff13-cef1-4dcf-aa2e-ee5b0b7dabeb", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "admin", "ADMIN", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7924435e-c36a-4fdc-8532-90c73575e265");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a311a857-78ec-44a6-836e-f1265ed18133");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b3a87c45-6456-4729-9e2d-d89bcf998322");
        }
    }
}
