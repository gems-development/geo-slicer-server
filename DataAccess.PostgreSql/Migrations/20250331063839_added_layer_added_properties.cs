using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataAccess.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class added_layer_added_properties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LayerId",
                table: "GeometryOriginals",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Properties",
                table: "GeometryOriginals",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Layers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Alias = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Layers", x => x.Id);
                });
            
            migrationBuilder.Sql("INSERT INTO \"Layers\" (\"Id\", \"Alias\") VALUES (0, 'DefaultLayer');");

            migrationBuilder.CreateIndex(
                name: "IX_GeometryOriginals_LayerId",
                table: "GeometryOriginals",
                column: "LayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Layers_Alias",
                table: "Layers",
                column: "Alias",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GeometryOriginals_Layers_LayerId",
                table: "GeometryOriginals",
                column: "LayerId",
                principalTable: "Layers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GeometryOriginals_Layers_LayerId",
                table: "GeometryOriginals");

            migrationBuilder.DropTable(
                name: "Layers");

            migrationBuilder.DropIndex(
                name: "IX_GeometryOriginals_LayerId",
                table: "GeometryOriginals");

            migrationBuilder.DropColumn(
                name: "LayerId",
                table: "GeometryOriginals");

            migrationBuilder.DropColumn(
                name: "Properties",
                table: "GeometryOriginals");
        }
    }
}
