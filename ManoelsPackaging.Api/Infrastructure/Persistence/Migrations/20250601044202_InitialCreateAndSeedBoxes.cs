using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManoelsPackaging.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateAndSeedBoxes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Boxes",
                columns: table => new
                {
                    TypeIdentifier = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DimensionHeight = table.Column<double>(type: "float", nullable: false),
                    DimensionLength = table.Column<double>(type: "float", nullable: false),
                    DimensionWidth = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boxes", x => x.TypeIdentifier);
                });

            migrationBuilder.InsertData(
                table: "Boxes",
                column: "TypeIdentifier",
                values: new object[]
                {
                    "Caixa 1",
                    "Caixa 2",
                    "Caixa 3"
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Boxes");
        }
    }
}
