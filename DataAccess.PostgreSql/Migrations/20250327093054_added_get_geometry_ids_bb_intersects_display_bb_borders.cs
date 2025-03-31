using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class added_get_geometry_ids_bb_intersects_display_bb_borders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION get_geometry_ids_intersects_display_bb_borders(polygon GEOMETRY)
                RETURNS SETOF integer AS $$
                DECLARE 
                    original_ids INTEGER[];
                    fragment RECORD;
                BEGIN
	                original_ids := '{}';
                  
                    -- Проверяем пересечения и записываем уникальные идентификаторы
                    FOR fragment IN 
                        SELECT ""Fragment"" AS fragment, ""GeometryOriginalId"" AS original_id
                        FROM ""GeometryFragments"" AS f 
                        WHERE f.""Fragment"" && polygon AND NOT (f.""Fragment"" @ polygon)
                    LOOP
                        IF NOT (fragment.original_id = ANY(original_ids)) THEN
                            IF ST_Intersects(fragment.fragment::geometry, polygon) THEN
                                original_ids := array_append(original_ids, fragment.original_id);
                                RETURN NEXT fragment.original_id;
                            END IF;
                        END IF;
                    END LOOP;

                    RETURN;
                END;
                $$ LANGUAGE plpgsql;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS get_geometry_ids_intersects_display_bb_borders;");
        }
    }
}
