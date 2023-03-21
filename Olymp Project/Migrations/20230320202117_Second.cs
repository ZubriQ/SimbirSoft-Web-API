using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Olymp_Project.Migrations
{
    public partial class Second : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Animal_Account",
                table: "Animal");

            migrationBuilder.DropForeignKey(
                name: "FK_Animal_Location",
                table: "Animal");

            migrationBuilder.DropForeignKey(
                name: "FK_AnimalType_Animal",
                table: "AnimalKind");

            migrationBuilder.DropForeignKey(
                name: "FK_AnimalType_Type",
                table: "AnimalKind");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitedLocation_Animal",
                table: "VisitedLocation");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitedLocation_Location",
                table: "VisitedLocation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AnimalType",
                table: "AnimalKind");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VisitedLocation",
                table: "VisitedLocation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Location",
                table: "Location");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Kind",
                table: "Kind");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Animal",
                table: "Animal");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Account",
                table: "Account");

            migrationBuilder.RenameTable(
                name: "VisitedLocation",
                newName: "VisitedLocations");

            migrationBuilder.RenameTable(
                name: "Location",
                newName: "Locations");

            migrationBuilder.RenameTable(
                name: "Kind",
                newName: "Kinds");

            migrationBuilder.RenameTable(
                name: "Animal",
                newName: "Animals");

            migrationBuilder.RenameTable(
                name: "Account",
                newName: "Accounts");

            migrationBuilder.RenameColumn(
                name: "KindId",
                table: "AnimalKind",
                newName: "KindsId");

            migrationBuilder.RenameColumn(
                name: "AnimalId",
                table: "AnimalKind",
                newName: "AnimalsId");

            migrationBuilder.RenameIndex(
                name: "IX_AnimalKind_KindId",
                table: "AnimalKind",
                newName: "IX_AnimalKind_KindsId");

            migrationBuilder.RenameIndex(
                name: "IX_VisitedLocation_LocationId",
                table: "VisitedLocations",
                newName: "IX_VisitedLocations_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_VisitedLocation_AnimalId",
                table: "VisitedLocations",
                newName: "IX_VisitedLocations_AnimalId");

            migrationBuilder.RenameIndex(
                name: "IX_Animal_ChippingLocationId",
                table: "Animals",
                newName: "IX_Animals_ChippingLocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Animal_ChipperId",
                table: "Animals",
                newName: "IX_Animals_ChipperId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "VisitDateTime",
                table: "VisitedLocations",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "VisitedLocations",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<double>(
                name: "Longitude",
                table: "Locations",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<double>(
                name: "Latitude",
                table: "Locations",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Locations",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Kinds",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Kinds",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<string>(
                name: "LifeStatus",
                table: "Animals",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "Animals",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeathDateTime",
                table: "Animals",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ChippingDateTime",
                table: "Animals",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<int>(
                name: "ChipperId",
                table: "Animals",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Animals",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Accounts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Accounts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Accounts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Accounts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(320)",
                oldMaxLength: 320);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Accounts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AnimalKind",
                table: "AnimalKind",
                columns: new[] { "AnimalsId", "KindsId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_VisitedLocations",
                table: "VisitedLocations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Locations",
                table: "Locations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Kinds",
                table: "Kinds",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Animals",
                table: "Animals",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Accounts",
                table: "Accounts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AnimalKind_Animals_AnimalsId",
                table: "AnimalKind",
                column: "AnimalsId",
                principalTable: "Animals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AnimalKind_Kinds_KindsId",
                table: "AnimalKind",
                column: "KindsId",
                principalTable: "Kinds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Animals_Accounts_ChipperId",
                table: "Animals",
                column: "ChipperId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Animals_Locations_ChippingLocationId",
                table: "Animals",
                column: "ChippingLocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VisitedLocations_Animals_AnimalId",
                table: "VisitedLocations",
                column: "AnimalId",
                principalTable: "Animals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VisitedLocations_Locations_LocationId",
                table: "VisitedLocations",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnimalKind_Animals_AnimalsId",
                table: "AnimalKind");

            migrationBuilder.DropForeignKey(
                name: "FK_AnimalKind_Kinds_KindsId",
                table: "AnimalKind");

            migrationBuilder.DropForeignKey(
                name: "FK_Animals_Accounts_ChipperId",
                table: "Animals");

            migrationBuilder.DropForeignKey(
                name: "FK_Animals_Locations_ChippingLocationId",
                table: "Animals");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitedLocations_Animals_AnimalId",
                table: "VisitedLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitedLocations_Locations_LocationId",
                table: "VisitedLocations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AnimalKind",
                table: "AnimalKind");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VisitedLocations",
                table: "VisitedLocations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Locations",
                table: "Locations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Kinds",
                table: "Kinds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Animals",
                table: "Animals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Accounts",
                table: "Accounts");

            migrationBuilder.RenameTable(
                name: "VisitedLocations",
                newName: "VisitedLocation");

            migrationBuilder.RenameTable(
                name: "Locations",
                newName: "Location");

            migrationBuilder.RenameTable(
                name: "Kinds",
                newName: "Kind");

            migrationBuilder.RenameTable(
                name: "Animals",
                newName: "Animal");

            migrationBuilder.RenameTable(
                name: "Accounts",
                newName: "Account");

            migrationBuilder.RenameColumn(
                name: "KindsId",
                table: "AnimalKind",
                newName: "KindId");

            migrationBuilder.RenameColumn(
                name: "AnimalsId",
                table: "AnimalKind",
                newName: "AnimalId");

            migrationBuilder.RenameIndex(
                name: "IX_AnimalKind_KindsId",
                table: "AnimalKind",
                newName: "IX_AnimalKind_KindId");

            migrationBuilder.RenameIndex(
                name: "IX_VisitedLocations_LocationId",
                table: "VisitedLocation",
                newName: "IX_VisitedLocation_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_VisitedLocations_AnimalId",
                table: "VisitedLocation",
                newName: "IX_VisitedLocation_AnimalId");

            migrationBuilder.RenameIndex(
                name: "IX_Animals_ChippingLocationId",
                table: "Animal",
                newName: "IX_Animal_ChippingLocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Animals_ChipperId",
                table: "Animal",
                newName: "IX_Animal_ChipperId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "VisitDateTime",
                table: "VisitedLocation",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "VisitedLocation",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<double>(
                name: "Longitude",
                table: "Location",
                type: "float",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<double>(
                name: "Latitude",
                table: "Location",
                type: "float",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Location",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Kind",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Kind",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<string>(
                name: "LifeStatus",
                table: "Animal",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "Animal",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeathDateTime",
                table: "Animal",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ChippingDateTime",
                table: "Animal",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<int>(
                name: "ChipperId",
                table: "Animal",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Animal",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Account",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Account",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Account",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Account",
                type: "nvarchar(320)",
                maxLength: 320,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Account",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AnimalType",
                table: "AnimalKind",
                columns: new[] { "AnimalId", "KindId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_VisitedLocation",
                table: "VisitedLocation",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Location",
                table: "Location",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Kind",
                table: "Kind",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Animal",
                table: "Animal",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Account",
                table: "Account",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Animal_Account",
                table: "Animal",
                column: "ChipperId",
                principalTable: "Account",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Animal_Location",
                table: "Animal",
                column: "ChippingLocationId",
                principalTable: "Location",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AnimalType_Animal",
                table: "AnimalKind",
                column: "AnimalId",
                principalTable: "Animal",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AnimalType_Type",
                table: "AnimalKind",
                column: "KindId",
                principalTable: "Kind",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VisitedLocation_Animal",
                table: "VisitedLocation",
                column: "AnimalId",
                principalTable: "Animal",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VisitedLocation_Location",
                table: "VisitedLocation",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id");
        }
    }
}
