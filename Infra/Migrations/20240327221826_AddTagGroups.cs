using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddTagGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TagGroup_Tenants_TenantId",
                table: "TagGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_TagTagGroup_TagGroup_TagGroupsId",
                table: "TagTagGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TagGroup",
                table: "TagGroup");

            migrationBuilder.RenameTable(
                name: "TagGroup",
                newName: "TagGroups");

            migrationBuilder.RenameIndex(
                name: "IX_TagGroup_TenantId",
                table: "TagGroups",
                newName: "IX_TagGroups_TenantId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TagGroups",
                table: "TagGroups",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TagGroups_Tenants_TenantId",
                table: "TagGroups",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TagTagGroup_TagGroups_TagGroupsId",
                table: "TagTagGroup",
                column: "TagGroupsId",
                principalTable: "TagGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TagGroups_Tenants_TenantId",
                table: "TagGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_TagTagGroup_TagGroups_TagGroupsId",
                table: "TagTagGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TagGroups",
                table: "TagGroups");

            migrationBuilder.RenameTable(
                name: "TagGroups",
                newName: "TagGroup");

            migrationBuilder.RenameIndex(
                name: "IX_TagGroups_TenantId",
                table: "TagGroup",
                newName: "IX_TagGroup_TenantId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TagGroup",
                table: "TagGroup",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TagGroup_Tenants_TenantId",
                table: "TagGroup",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TagTagGroup_TagGroup_TagGroupsId",
                table: "TagTagGroup",
                column: "TagGroupsId",
                principalTable: "TagGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
