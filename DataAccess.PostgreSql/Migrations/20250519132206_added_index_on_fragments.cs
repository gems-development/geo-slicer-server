using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class added_index_on_fragments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS btree_gist");
            migrationBuilder.Sql(
                "CREATE INDEX idx_geometryfragments_geometryoriginalid_fragment ON \"GeometryFragments\" USING GIST (\"GeometryOriginalId\", \"Fragment\")");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP INDEX IF EXISTS idx_geometryfragments_geometryoriginalid_fragment");
        }
    }
}
