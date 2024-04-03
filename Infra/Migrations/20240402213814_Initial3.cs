using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infra.Migrations
{
    /// <inheritdoc />
    public partial class Initial3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserTenant_Tenants_TenantId",
                table: "ApplicationUserTenant");

            migrationBuilder.DropForeignKey(
                name: "FK_Tenants_AspNetUsers_ApplicationUserId",
                table: "Tenants");

            migrationBuilder.DropTable(
                name: "ApplicationUserTenant1");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_ApplicationUserId",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Tenants");

            migrationBuilder.RenameColumn(
                name: "TenantId",
                table: "ApplicationUserTenant",
                newName: "AvailableTenantsId");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationUserTenant_TenantId",
                table: "ApplicationUserTenant",
                newName: "IX_ApplicationUserTenant_AvailableTenantsId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserTenant_Tenants_AvailableTenantsId",
                table: "ApplicationUserTenant",
                column: "AvailableTenantsId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserTenant_Tenants_AvailableTenantsId",
                table: "ApplicationUserTenant");

            migrationBuilder.RenameColumn(
                name: "AvailableTenantsId",
                table: "ApplicationUserTenant",
                newName: "TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationUserTenant_AvailableTenantsId",
                table: "ApplicationUserTenant",
                newName: "IX_ApplicationUserTenant_TenantId");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Tenants",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ApplicationUserTenant1",
                columns: table => new
                {
                    ApplicationUser1Id = table.Column<string>(type: "text", nullable: false),
                    Tenant1Id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserTenant1", x => new { x.ApplicationUser1Id, x.Tenant1Id });
                    table.ForeignKey(
                        name: "FK_ApplicationUserTenant1_AspNetUsers_ApplicationUser1Id",
                        column: x => x.ApplicationUser1Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserTenant1_Tenants_Tenant1Id",
                        column: x => x.Tenant1Id,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_ApplicationUserId",
                table: "Tenants",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserTenant1_Tenant1Id",
                table: "ApplicationUserTenant1",
                column: "Tenant1Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserTenant_Tenants_TenantId",
                table: "ApplicationUserTenant",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tenants_AspNetUsers_ApplicationUserId",
                table: "Tenants",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
