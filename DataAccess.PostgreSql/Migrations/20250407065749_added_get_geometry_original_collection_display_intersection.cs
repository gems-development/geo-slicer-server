using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class added_get_geometry_original_collection_display_intersection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION get_geometry_original_collection_display_intersection(polygon GEOMETRY)
                RETURNS SETOF ""GeometryOriginals"" AS $$
                DECLARE 
                    original_ids_in_display INTEGER[];
	                original_ids_fragments_in_display INTEGER[];
	                original_ids_fragments_intersects_display INTEGER[];
                    fragment ""GeometryFragments""%ROWTYPE;
	                temp_record ""GeometryOriginals""%ROWTYPE;
                BEGIN
                    original_ids_in_display := '{}';
	                original_ids_fragments_in_display := '{}';
	                original_ids_fragments_intersects_display := '{}';
                  
                    FOR temp_record IN 
                        SELECT *
                        FROM ""GeometryOriginals"" AS f 
                        WHERE f.""Data"" @ polygon
                    LOOP
	                     original_ids_in_display := array_append(original_ids_in_display, temp_record.""Id"");
	                     RETURN NEXT temp_record;
                    END LOOP;

	                SELECT ARRAY (SELECT DISTINCT f.""GeometryOriginalId""
                        FROM ""GeometryFragments"" AS f 
                        WHERE f.""Fragment"" @ polygon AND NOT (f.""GeometryOriginalId"" = ANY(original_ids_in_display)))
	                INTO original_ids_fragments_in_display;

                    FOR fragment IN 
                        SELECT *
                        FROM ""GeometryFragments"" AS f 
                        WHERE (f.""Fragment"" && polygon AND NOT f.""Fragment"" @ polygon) AND NOT (f.""GeometryOriginalId"" = ANY(original_ids_fragments_in_display))
                    LOOP
                        IF NOT (fragment.""GeometryOriginalId"" = ANY(original_ids_fragments_intersects_display)) AND (ST_Intersects(fragment.""Fragment""::geometry, polygon)) THEN
                            original_ids_fragments_intersects_display := array_append(original_ids_fragments_intersects_display, fragment.""GeometryOriginalId"");
                        END IF;
                    END LOOP;

	                FOR temp_record IN 
                        SELECT *
                        FROM ""GeometryOriginals"" AS f 
                        WHERE f.""Id"" = ANY(original_ids_fragments_in_display) OR f.""Id"" = ANY(original_ids_fragments_intersects_display)
                    LOOP
	                     temp_record.""Data"" = ST_Intersection(temp_record.""Data""::geometry, polygon);
	                     RETURN NEXT temp_record;
                    END LOOP;

                    RETURN;
                END;
                $$ LANGUAGE plpgsql;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS get_geometry_original_collection_display_intersection;");
        }
    }
}
