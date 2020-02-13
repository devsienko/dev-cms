using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace devcms.Migrations
{
    public partial class InitMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NotificationRedirectionEmail = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContentTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileAttrValue",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FileName = table.Column<string>(nullable: true),
                    ContentType = table.Column<string>(nullable: true),
                    Bytes = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileAttrValue", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(maxLength: 255, nullable: true),
                    Password = table.Column<string>(maxLength: 255, nullable: true),
                    PasswordSalt = table.Column<string>(maxLength: 255, nullable: true),
                    RegistrationStatus = table.Column<string>(maxLength: 255, nullable: true),
                    EmailStatus = table.Column<string>(maxLength: 255, nullable: true),
                    SecurityQuestion = table.Column<string>(maxLength: 255, nullable: true),
                    SecurityAnswer = table.Column<string>(maxLength: 255, nullable: true),
                    RegistrationDateTime = table.Column<DateTime>(nullable: true),
                    FailedLoginAttemptDateTime = table.Column<DateTime>(nullable: true),
                    FailedLoginAttemptCounter = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Content",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EntityTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Content", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Content_ContentTypes_EntityTypeId",
                        column: x => x.EntityTypeId,
                        principalTable: "ContentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContentAttrs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    AttrType = table.Column<int>(nullable: false),
                    EntityTypeId = table.Column<int>(nullable: true),
                    ContentTypeId = table.Column<int>(nullable: false),
                    Required = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentAttrs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentAttrs_ContentTypes_EntityTypeId",
                        column: x => x.EntityTypeId,
                        principalTable: "ContentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AttrValues",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Value = table.Column<string>(nullable: true),
                    ValueAsFileId = table.Column<int>(nullable: true),
                    AttrId = table.Column<int>(nullable: false),
                    EntityId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttrValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttrValues_ContentAttrs_AttrId",
                        column: x => x.AttrId,
                        principalTable: "ContentAttrs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttrValues_Content_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Content",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttrValues_FileAttrValue_ValueAsFileId",
                        column: x => x.ValueAsFileId,
                        principalTable: "FileAttrValue",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttrValues_AttrId",
                table: "AttrValues",
                column: "AttrId");

            migrationBuilder.CreateIndex(
                name: "IX_AttrValues_EntityId",
                table: "AttrValues",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_AttrValues_ValueAsFileId",
                table: "AttrValues",
                column: "ValueAsFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Content_EntityTypeId",
                table: "Content",
                column: "EntityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentAttrs_EntityTypeId",
                table: "ContentAttrs",
                column: "EntityTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationSettings");

            migrationBuilder.DropTable(
                name: "AttrValues");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "ContentAttrs");

            migrationBuilder.DropTable(
                name: "Content");

            migrationBuilder.DropTable(
                name: "FileAttrValue");

            migrationBuilder.DropTable(
                name: "ContentTypes");
        }
    }
}
