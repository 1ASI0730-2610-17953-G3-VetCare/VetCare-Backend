using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using VetCare.clinicManagement.infrastructure.persistence.EFC.context;

#nullable disable

namespace VetCare.clinicManagement.infrastructure.persistence.EFC.migrations
{
    [DbContext(typeof(ClinicContext))]
    [Migration("20260618140000_AddHospitalization")]
    public partial class AddHospitalization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "hospitalization_admissions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    patient_id = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    diagnosis = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    admission_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    treatments_json = table.Column<string>(type: "text", nullable: false),
                    discharged_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hospitalization_admissions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "hospitalization_tasks",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    patient_id = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    title = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    task_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    task_time = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    completed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hospitalization_tasks", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "hospitalization_tasks");
            migrationBuilder.DropTable(name: "hospitalization_admissions");
        }
    }
}
