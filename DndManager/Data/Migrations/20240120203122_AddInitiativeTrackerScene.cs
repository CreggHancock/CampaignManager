using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DndManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddInitiativeTrackerScene : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Languages_Characters_CharacterId",
                table: "Languages");

            migrationBuilder.DropIndex(
                name: "IX_Languages_CharacterId",
                table: "Languages");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "CharacterClasses");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Abilities");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Abilities");

            migrationBuilder.RenameColumn(
                name: "CharacterId",
                table: "Languages",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "MaxUses",
                table: "Abilities",
                newName: "CreatedBy");

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "SpellSlots",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "SpellSlots",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsCustom",
                table: "Spells",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "ProfiencyBonuses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "ProfiencyBonuses",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Languages",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsCustom",
                table: "Languages",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "InventoryItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "InventoryItems",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "Characters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Characters",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ClassDefinitionId",
                table: "CharacterClasses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "CharacterClasses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "CharacterClasses",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AddColumn<int>(
                name: "AbilityDefinitionId",
                table: "Abilities",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Abilities",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "AbilityDefinition",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaxUses = table.Column<int>(type: "integer", nullable: false),
                    IsCustom = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbilityDefinition", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CharacterLanguage",
                columns: table => new
                {
                    CharactersId = table.Column<int>(type: "integer", nullable: false),
                    LanguagesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterLanguage", x => new { x.CharactersId, x.LanguagesId });
                    table.ForeignKey(
                        name: "FK_CharacterLanguage_Characters_CharactersId",
                        column: x => x.CharactersId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterLanguage_Languages_LanguagesId",
                        column: x => x.LanguagesId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClassDefinition",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    IsCustom = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassDefinition", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubClassDefinition",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClassDefinitionId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsCustom = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubClassDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubClassDefinition_ClassDefinition_ClassDefinitionId",
                        column: x => x.ClassDefinitionId,
                        principalTable: "ClassDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubClass",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClassId = table.Column<int>(type: "integer", nullable: false),
                    SubClassDefinitionId = table.Column<int>(type: "integer", nullable: false),
                    CharacterClassId = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubClass", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubClass_CharacterClasses_CharacterClassId",
                        column: x => x.CharacterClassId,
                        principalTable: "CharacterClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubClass_SubClassDefinition_SubClassDefinitionId",
                        column: x => x.SubClassDefinitionId,
                        principalTable: "SubClassDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterClasses_ClassDefinitionId",
                table: "CharacterClasses",
                column: "ClassDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_Abilities_AbilityDefinitionId",
                table: "Abilities",
                column: "AbilityDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterLanguage_LanguagesId",
                table: "CharacterLanguage",
                column: "LanguagesId");

            migrationBuilder.CreateIndex(
                name: "IX_SubClass_CharacterClassId",
                table: "SubClass",
                column: "CharacterClassId");

            migrationBuilder.CreateIndex(
                name: "IX_SubClass_SubClassDefinitionId",
                table: "SubClass",
                column: "SubClassDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_SubClassDefinition_ClassDefinitionId",
                table: "SubClassDefinition",
                column: "ClassDefinitionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Abilities_AbilityDefinition_AbilityDefinitionId",
                table: "Abilities",
                column: "AbilityDefinitionId",
                principalTable: "AbilityDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterClasses_ClassDefinition_ClassDefinitionId",
                table: "CharacterClasses",
                column: "ClassDefinitionId",
                principalTable: "ClassDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Abilities_AbilityDefinition_AbilityDefinitionId",
                table: "Abilities");

            migrationBuilder.DropForeignKey(
                name: "FK_CharacterClasses_ClassDefinition_ClassDefinitionId",
                table: "CharacterClasses");

            migrationBuilder.DropTable(
                name: "AbilityDefinition");

            migrationBuilder.DropTable(
                name: "CharacterLanguage");

            migrationBuilder.DropTable(
                name: "SubClass");

            migrationBuilder.DropTable(
                name: "SubClassDefinition");

            migrationBuilder.DropTable(
                name: "ClassDefinition");

            migrationBuilder.DropIndex(
                name: "IX_CharacterClasses_ClassDefinitionId",
                table: "CharacterClasses");

            migrationBuilder.DropIndex(
                name: "IX_Abilities_AbilityDefinitionId",
                table: "Abilities");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "SpellSlots");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "SpellSlots");

            migrationBuilder.DropColumn(
                name: "IsCustom",
                table: "Spells");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ProfiencyBonuses");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "ProfiencyBonuses");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Languages");

            migrationBuilder.DropColumn(
                name: "IsCustom",
                table: "Languages");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "ClassDefinitionId",
                table: "CharacterClasses");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "CharacterClasses");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "CharacterClasses");

            migrationBuilder.DropColumn(
                name: "AbilityDefinitionId",
                table: "Abilities");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Abilities");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Languages",
                newName: "CharacterId");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Abilities",
                newName: "MaxUses");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "CharacterClasses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Abilities",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Abilities",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_CharacterId",
                table: "Languages",
                column: "CharacterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Languages_Characters_CharacterId",
                table: "Languages",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
