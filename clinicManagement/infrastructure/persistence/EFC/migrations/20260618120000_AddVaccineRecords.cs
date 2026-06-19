using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using VetCare.clinicManagement.infrastructure.persistence.EFC.context;

#nullable disable

namespace VetCare.clinicManagement.infrastructure.persistence.EFC.migrations
{
    /// <inheritdoc />
    [DbContext(typeof(ClinicContext))]
    [Migration("20260618120000_AddVaccineRecords")]
    public partial class AddVaccineRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "vaccine_records",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    patient_id = table.Column<int>(type: "integer", nullable: false),
                    vaccine_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    disease = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    last_application = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    next_dose = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    product_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vaccine_records", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "vaccine_records");
        }
    }
}
