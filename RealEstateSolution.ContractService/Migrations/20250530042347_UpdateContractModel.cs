using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateSolution.ContractService.Migrations
{
    /// <inheritdoc />
    public partial class UpdateContractModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientRequirements_Client_ClientId",
                table: "ClientRequirements");

            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Client_ClientId",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Client_ClientId1",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Property_PropertyId1",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_Matching_Client_ClientId",
                table: "Matching");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_ClientId1",
                table: "Contracts");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_PropertyId1",
                table: "Contracts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Client",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "ClientId1",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "PropertyId1",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "Remark",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "Terms",
                table: "Contracts");

            migrationBuilder.RenameTable(
                name: "Client",
                newName: "Clients");

            migrationBuilder.RenameColumn(
                name: "UpdateTime",
                table: "Contracts",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Contracts",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "SignTime",
                table: "Contracts",
                newName: "SignDate");

            migrationBuilder.RenameColumn(
                name: "CompleteTime",
                table: "Contracts",
                newName: "ExpiryDate");

            migrationBuilder.RenameColumn(
                name: "ClientId",
                table: "Contracts",
                newName: "PartyBId");

            migrationBuilder.RenameColumn(
                name: "CancelTime",
                table: "Contracts",
                newName: "EffectiveDate");

            migrationBuilder.RenameIndex(
                name: "IX_Contracts_ClientId",
                table: "Contracts",
                newName: "IX_Contracts_PartyBId");

            migrationBuilder.AlterColumn<string>(
                name: "ContractNumber",
                table: "Contracts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Contracts",
                type: "ntext",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Contracts",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Contracts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Contracts",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PartyAId",
                table: "Contracts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Contracts",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Source",
                table: "Clients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Clients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Clients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Clients",
                table: "Clients",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_PartyAId",
                table: "Contracts",
                column: "PartyAId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientRequirements_Clients_ClientId",
                table: "ClientRequirements",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Clients_PartyAId",
                table: "Contracts",
                column: "PartyAId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Clients_PartyBId",
                table: "Contracts",
                column: "PartyBId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Matching_Clients_ClientId",
                table: "Matching",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientRequirements_Clients_ClientId",
                table: "ClientRequirements");

            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Clients_PartyAId",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Clients_PartyBId",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_Matching_Clients_ClientId",
                table: "Matching");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_PartyAId",
                table: "Contracts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Clients",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "PartyAId",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Clients");

            migrationBuilder.RenameTable(
                name: "Clients",
                newName: "Client");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Contracts",
                newName: "UpdateTime");

            migrationBuilder.RenameColumn(
                name: "SignDate",
                table: "Contracts",
                newName: "SignTime");

            migrationBuilder.RenameColumn(
                name: "PartyBId",
                table: "Contracts",
                newName: "ClientId");

            migrationBuilder.RenameColumn(
                name: "ExpiryDate",
                table: "Contracts",
                newName: "CompleteTime");

            migrationBuilder.RenameColumn(
                name: "EffectiveDate",
                table: "Contracts",
                newName: "CancelTime");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Contracts",
                newName: "StartDate");

            migrationBuilder.RenameIndex(
                name: "IX_Contracts_PartyBId",
                table: "Contracts",
                newName: "IX_Contracts_ClientId");

            migrationBuilder.AlterColumn<string>(
                name: "ContractNumber",
                table: "Contracts",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<int>(
                name: "ClientId1",
                table: "Contracts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "Contracts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Contracts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "Contracts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PropertyId1",
                table: "Contracts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remark",
                table: "Contracts",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Terms",
                table: "Contracts",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Client",
                table: "Client",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ClientId1",
                table: "Contracts",
                column: "ClientId1");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_PropertyId1",
                table: "Contracts",
                column: "PropertyId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientRequirements_Client_ClientId",
                table: "ClientRequirements",
                column: "ClientId",
                principalTable: "Client",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Client_ClientId",
                table: "Contracts",
                column: "ClientId",
                principalTable: "Client",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Client_ClientId1",
                table: "Contracts",
                column: "ClientId1",
                principalTable: "Client",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Property_PropertyId1",
                table: "Contracts",
                column: "PropertyId1",
                principalTable: "Property",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Matching_Client_ClientId",
                table: "Matching",
                column: "ClientId",
                principalTable: "Client",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
