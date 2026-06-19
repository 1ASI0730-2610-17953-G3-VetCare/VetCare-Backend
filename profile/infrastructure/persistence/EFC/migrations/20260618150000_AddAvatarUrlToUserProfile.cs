using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using VetCare.profile.infrastructure.persistence.EFC.context;

#nullable disable

namespace VetCare.profile.infrastructure.persistence.EFC.migrations
{
    [DbContext(typeof(ProfileContext))]
    [Migration("20260618150000_AddAvatarUrlToUserProfile")]
    public partial class AddAvatarUrlToUserProfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "avatar_url",
                table: "user_profiles",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "avatar_url",
                table: "user_profiles");
        }
    }
}
