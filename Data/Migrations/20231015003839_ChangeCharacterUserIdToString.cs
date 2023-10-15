﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DndManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCharacterUserIdToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Characters",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Characters",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
