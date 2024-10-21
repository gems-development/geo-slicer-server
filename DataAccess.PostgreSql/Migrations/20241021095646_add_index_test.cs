using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class add_index_test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_GeometryOriginals_Data",
                table: "GeometryOriginals",
                column: "Data")
                .Annotation("Npgsql:IndexMethod", "GIST");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GeometryOriginals_Data",
                table: "GeometryOriginals");
        }
    }
}
