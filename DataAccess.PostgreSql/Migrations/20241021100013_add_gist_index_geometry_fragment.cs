using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class add_gist_index_geometry_fragment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_GeometryFragments_Fragment",
                table: "GeometryFragments",
                column: "Fragment")
                .Annotation("Npgsql:IndexMethod", "GIST");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GeometryFragments_Fragment",
                table: "GeometryFragments");
        }
    }
}
