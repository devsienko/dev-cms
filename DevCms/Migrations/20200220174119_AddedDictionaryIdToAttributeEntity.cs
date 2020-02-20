using Microsoft.EntityFrameworkCore.Migrations;

namespace devcms.Migrations
{
    public partial class AddedDictionaryIdToAttributeEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DictionaryId",
                table: "ContentAttrs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DictionaryId",
                table: "ContentAttrs");
        }
    }
}
