using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class edited_get_intersecting_geometry_by_search_fragments : Migration
    {
         /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TYPE IF EXISTS fragment_original_id;");
            
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TYPE IF EXISTS fragment_original_id;");
            migrationBuilder.Sql(@"
                CREATE TYPE fragment_original_id AS (fragment POLYGON, original_id INT);");
            
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION get_intersecting_geometry_by_search_fragments(polygon GEOMETRY)
                RETURNS GEOMETRY AS $$
                DECLARE 
                    original_ids INTEGER[];
                    fragments fragment_original_id[];
                    originals GEOMETRY[];
                    fragment fragment_original_id;
                BEGIN
	                original_ids := '{}';
                    -- Находим фрагменты, пересекающиеся с входным полигоном
                    SELECT ARRAY(
                        SELECT ROW(""Fragment"", ""GeometryOriginalId"")::fragment_original_id 
                        FROM ""GeometryFragments"" AS f 
                        WHERE f.""Fragment"" && polygon
                    ) 
                    INTO fragments;    
                    -- Проверяем пересечения и записываем уникальные идентификаторы
                    FOREACH fragment IN ARRAY fragments LOOP
                        IF NOT (fragment.original_id = ANY(original_ids)) THEN
                            IF ST_Intersects(fragment.fragment::geometry, polygon) THEN
                                original_ids := array_append(original_ids, fragment.original_id);
                            END IF;
                        END IF;
                    END LOOP;

                    -- Извлекаем пересечения
                    SELECT ARRAY(
                        SELECT ST_Intersection(f.""Data"", polygon) 
                        FROM ""GeometryOriginals"" AS f 
                        WHERE f.""Id"" = ANY(original_ids)
                    ) 
                    INTO originals;

                    -- Объединяем геометрию и возвращаем
                    RETURN ST_Collect(originals);
                END;
                $$ LANGUAGE plpgsql;
            ");
        }
    }
}
