using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataAccess.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "GeometryOriginals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Data = table.Column<Polygon>(type: "geometry", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeometryOriginals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GeometryFragments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Fragment = table.Column<Polygon>(type: "geometry", nullable: false),
                    NonRenderingBorder = table.Column<MultiLineString>(type: "geometry", nullable: false),
                    GeometryOriginalId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeometryFragments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeometryFragments_GeometryOriginals_GeometryOriginalId",
                        column: x => x.GeometryOriginalId,
                        principalTable: "GeometryOriginals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GeometryFragments_GeometryOriginalId",
                table: "GeometryFragments",
                column: "GeometryOriginalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GeometryFragments");

            migrationBuilder.DropTable(
                name: "GeometryOriginals");
        }
    }
}
