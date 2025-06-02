using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ManoelsPackaging.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateAndSeedBoxes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DimensionsSet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Height = table.Column<double>(type: "float", nullable: false),
                    Width = table.Column<double>(type: "float", nullable: false),
                    Length = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DimensionsSet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Boxes",
                columns: table => new
                {
                    TypeIdentifier = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DimensionsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boxes", x => x.TypeIdentifier);
                    table.ForeignKey(
                        name: "FK_Boxes_DimensionsSet_DimensionsId",
                        column: x => x.DimensionsId,
                        principalTable: "DimensionsSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "DimensionsSet",
                columns: new[] { "Id", "Height", "Length", "Width" },
                values: new object[,]
                {
                    { 1, 30.0, 80.0, 40.0 },
                    { 2, 80.0, 40.0, 50.0 },
                    { 3, 50.0, 60.0, 80.0 }
                });

            migrationBuilder.InsertData(
                table: "Boxes",
                columns: new[] { "TypeIdentifier", "DimensionsId" },
                values: new object[,]
                {
                    { "Caixa 1", 1 },
                    { "Caixa 2", 2 },
                    { "Caixa 3", 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Boxes_DimensionsId",
                table: "Boxes",
                column: "DimensionsId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Boxes");

            migrationBuilder.DropTable(
                name: "DimensionsSet");
        }
    }
}
