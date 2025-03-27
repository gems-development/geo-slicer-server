using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class new_version_get_intersecting_geometry_by_search_fragments : Migration
    {
         /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS get_intersecting_geometry_by_search_fragments;");
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION get_intersecting_geometry_by_search_fragments(polygon GEOMETRY)
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
                        WHERE f.""Fragment"" && polygon
                    LOOP
                        IF NOT (fragment.original_id = ANY(original_ids)) THEN
                            IF fragment.fragment @ polygon OR ST_Intersects(fragment.fragment::geometry, polygon) THEN
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
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS get_intersecting_geometry_by_search_fragments;");
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION get_intersecting_geometry_by_search_fragments(polygon GEOMETRY)
                RETURNS GEOMETRY AS $$
                DECLARE 
                    original_ids INTEGER[];
                    flag BOOLEAN;
	                result RECORD; 
	                fragment GEOMETRY;
	                collected_geometry GEOMETRY; 
                BEGIN
                    original_ids := '{}';
                    flag := false;
                    FOR result IN 
                        SELECT ""GeometryOriginalId"" AS original_id, array_agg(""Fragment"") AS fragments
                        FROM ""GeometryFragments""
                        GROUP BY original_id
                    LOOP
                        FOREACH fragment IN ARRAY result.fragments LOOP
                            IF ST_Intersects(fragment::GEOMETRY, polygon) THEN
                                flag := true;
                                EXIT;
                            END IF;
                        END LOOP;
                        IF flag THEN
                            original_ids := array_append(original_ids, result.original_id);
                            flag := false;
                        END IF;
                    END LOOP;

                    SELECT ST_Collect(ST_Intersection(f.""Data"", polygon)) 
                    INTO collected_geometry
                    FROM ""GeometryOriginals"" AS f 
                    WHERE f.""Id"" = ANY(original_ids);

                    RETURN collected_geometry;
                END;
                $$ LANGUAGE plpgsql;
            ");
        }
    }
}
