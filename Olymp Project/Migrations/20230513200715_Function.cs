using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Olymp_Project.Migrations
{
    public partial class Function : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR REPLACE FUNCTION GetAllPaths()
                RETURNS TABLE
                (
                    Id BIGINT,
                    StartLocationId BIGINT,
                    EndLocationId BIGINT,
                    Weight DOUBLE PRECISION,
                    IsReversed BOOLEAN
                )
                AS $$
                BEGIN
                    RETURN QUERY SELECT * FROM ""Paths"";
                END;
                $$ LANGUAGE plpgsql;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS GetAllPaths()");
        }
    }
}
