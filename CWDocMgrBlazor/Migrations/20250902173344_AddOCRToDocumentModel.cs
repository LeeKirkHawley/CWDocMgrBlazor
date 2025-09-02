using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CWDocMgrBlazor.Migrations
{
    /// <inheritdoc />
    public partial class AddOCRToDocumentModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OCRText",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OCRText",
                table: "Documents");
        }
    }
}
