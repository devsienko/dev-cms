using Microsoft.EntityFrameworkCore.Migrations;

namespace devcms.Migrations
{
    public partial class AddedDictionaryItemIdToAttrValueEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DictionaryItemId",
                table: "AttrValues",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DictionaryItemId",
                table: "AttrValues");
        }
    }
}
